using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityExtensions;
using UnityExtensions.InspectInlineExamples;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif



namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{

        [Header("地面判定")]
		[Tooltip("地面に乗ってるかチェック用　切り替えて使わない")]
		public bool Grounded = true;

        [Header("Cinemachine")]
		[Tooltip("Cinemachineのターゲットオブジェクト")]
		public GameObject CinemachineCameraTarget;

        [Header("Player")] 
		[Tooltip("プロパティ設定場所"), InspectInline(canEditRemoteTarget = true)]
        public PlayerProperty playerProperty;

        [Header("Player")]
        public Transform SwordGrip;
		public Transform SwordRotator;
		public Collider SwordCollider;
		public ParticleSystem SwordTrail;
		public AudioSource audioSource;
        public GameObject PrefabAttack;
		public GameObject PrefabDeath;

        [Header("ゲームパッドのバイブレーション設定")]
		[Tooltip("敵が近づいたとき")]
		public bool NearEnemy = true;
        [Tooltip("敵に近いとき敵が歩いていたら敵の足音が聞こえる")] 
        public bool EnemyFootstep = false;
        [Tooltip("振動検知の距離設定をAudioSoruceの設定に合わせる")] 
        public bool VibrationAudioDependency = true;

        [Tooltip("振動が始まる距離")]
        public float MinDistance = 0.0f;
        [Tooltip("振動が始まる距離")]
        public float MaxDistance = 15.0f;

		[Header("自動設定されるやつ")]
        public int PlayerIndex;
        public PlayerManagement PlayerManagement;
        public Gamepad gamepad = null;

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;
		private bool _isAttacking;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		private PlayerInput _playerInput;
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;
        private MatchManager matchManager;

        private bool _isJump = false;

		//敵情報
        private Transform enemy;
        private CharacterController enemyCharacterController;

		private const float _threshold = 0.01f;
		
#if UNITY_EDITOR
		private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";
#endif

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
			_playerInput = GetComponent<PlayerInput>();

            if (PlayerManagement != null)
            {
                matchManager = PlayerManagement.matchManager;

				GameObject enemyobj = PlayerManagement.GetEnemy(PlayerIndex);
                if (enemyobj != null)
                {
                    enemy = enemyobj.transform;
                    enemyCharacterController = enemyobj.GetComponent<CharacterController>();
				}
                
            }

            // reset our timeouts on start
			_jumpTimeoutDelta = playerProperty.JumpTimeout;
			_fallTimeoutDelta = playerProperty.FallTimeout;

			//オーディオの設定を入れる
            if (VibrationAudioDependency)
            {
                MaxDistance = audioSource.maxDistance;
                MinDistance = audioSource.minDistance;
            }

			//ゲームパッドデバイス取得
            foreach (var device in _playerInput.devices)
            {
                if (device is Gamepad pad)
                {
                    gamepad = pad;
					break;
				}
            }
        }

		private void Update()
		{
           

			JumpAndGravity();
			GroundedCheck();
			Move();
			Attack();
			ControllerVibration();

           
		}

		private void LateUpdate()
		{
            CameraRotation();
		}

		public void Die(){
			Destroy(gameObject);
			var effect = Instantiate(PrefabDeath, transform.position, Quaternion.identity);
			Destroy(effect, 1.5f);
			matchManager.PlayerDied();
		}

		private void GroundedCheck()
		{
			//ジャンプ中は判定しない
			if(_isJump)
				return;

			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - playerProperty.GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, playerProperty.GroundedRadius, playerProperty.GroundLayers, QueryTriggerInteraction.Ignore);
        }

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = 1.0f;

#if UNITY_EDITOR
				//Don't multiply mouse input by Time.deltaTime
				deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
#else
				deltaTimeMultiplier = Time.deltaTime;

#endif
				_cinemachineTargetPitch += _input.look.y * playerProperty.RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * playerProperty.RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, playerProperty.BottomClamp, playerProperty.TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? playerProperty.SprintSpeed : playerProperty.MoveSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * playerProperty.SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
            if (Grounded)
			{
                // 落下クールタイムリセット
				_fallTimeoutDelta = playerProperty.FallTimeout;

                // 上下の加速度がおかしな数値にならないように抑制する
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

                // ジャンプ
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// H * -2 * G の平方根 = 望みの高さに到達するために必要な速度
					_verticalVelocity = Mathf.Sqrt(playerProperty.JumpHeight * -2f * playerProperty.Gravity);
                    _isJump = true;
                    Grounded = false;
                }

				// ジャンプクールタイム
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
                }
            }
			else
			{
				// ジャンプクールタイムリセット
				_jumpTimeoutDelta = playerProperty.JumpTimeout;

				// 落下クールタイム
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				//上昇中に天井に当たった場合即時落下する
                if (_controller.velocity.y == 0.0f)
                {
                    _isJump = false;
                    _verticalVelocity = 0.0f;
                }

				//ジャンプ入力受け付けるようにする
                _input.jump = false;
			}


            // 重力で落とす
			if (_verticalVelocity < _terminalVelocity)
			{
                _verticalVelocity += playerProperty.Gravity * Time.deltaTime;
            }
		}

		private void Attack()
		{
			if (!_input.attack) return;
			_input.attack = false;
			if (_isAttacking) return;
			StartCoroutine(DoAttack());
		}

		private IEnumerator DoAttack()
		{
			_isAttacking = true;
			var originalGripPos = SwordGrip.localPosition;
			SwordGrip.DOLocalMove(Vector3.zero, playerProperty.AttackPopOutSpeed).WaitForCompletion();
			yield return SwordGrip.DOLocalRotate(new Vector3(0, 90, 70), playerProperty.AttackPopOutSpeed).WaitForCompletion();

			Instantiate(PrefabAttack, transform.position, Quaternion.identity);
			SwordCollider.enabled = true;
			SwordTrail.Play();
			audioSource.PlayOneShot(playerProperty.seSwing);
			yield return SwordRotator.DOLocalRotate(new Vector3(0, -360, 0), playerProperty.AttaclSpeed, RotateMode.FastBeyond360).WaitForCompletion();

			SwordTrail.Stop();
			SwordGrip.DOLocalMove(originalGripPos, playerProperty.AttackPopOutSpeed).WaitForCompletion();
			yield return SwordGrip.DOLocalRotate(Vector3.zero, playerProperty.AttackPopOutSpeed).WaitForCompletion();

			yield return null;

			SwordRotator.localRotation = Quaternion.identity;
			SwordCollider.enabled = false;
			_isAttacking = false;
		}

        private void ControllerVibration()
        {
            //パッドが無かったら何もしない
			if (gamepad == null)
				return;

			//敵がいない場合振動をオフにして終わる
            if (enemy == null)
            {
                gamepad.SetMotorSpeeds(0.0f, 0.0f);
                return;
			}

			//敵に近づいてるとき
            if (NearEnemy)
            {
				NearEnemyProcess();
            }

			//敵に近づいててかつ敵が歩いてるとき
            if (EnemyFootstep)
            {
				EnemyFootstepProcess();
            }
        }

        private void NearEnemyProcess()
        {
			//距離で振動
			DistanceVibration();
		}

        private void EnemyFootstepProcess()
        {
			//歩いてたら振動
            Vector2 vec = new Vector2(enemyCharacterController.velocity.x, enemyCharacterController.velocity.z);
            if (vec.magnitude < 0.1f)
            {
                gamepad.SetMotorSpeeds(0.0f, 0.0f);
                return;
			}

			//振動
            DistanceVibration();
        }

        private void DistanceVibration()
        {
			//距離に合わせて振動させる
            float mag = 1.0f - (Mathf.Min((enemy.position - transform.position).magnitude, MaxDistance) / MaxDistance);
            gamepad.SetMotorSpeeds(mag, mag);
		}

        private static Vector2 ClampVector2(Vector2 clamp, float max, float min)
        {
            return new Vector2(Mathf.Clamp(clamp.x, min, max), Mathf.Clamp(clamp.y, min, max));
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}
    }
}