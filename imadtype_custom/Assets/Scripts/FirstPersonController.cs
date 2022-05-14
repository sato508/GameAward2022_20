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

        [Header("�n�ʔ���")]
		[Tooltip("�n�ʂɏ���Ă邩�`�F�b�N�p�@�؂�ւ��Ďg��Ȃ�")]
		public bool Grounded = true;

        [Header("Cinemachine")]
		[Tooltip("Cinemachine�̃^�[�Q�b�g�I�u�W�F�N�g")]
		public GameObject CinemachineCameraTarget;

        [Header("Player")] 
		[Tooltip("�v���p�e�B�ݒ�ꏊ"), InspectInline(canEditRemoteTarget = true)]
        public PlayerProperty playerProperty;

        [Header("Player")]
        public Transform SwordGrip;
		public Transform SwordRotator;
		public Collider SwordCollider;
		public ParticleSystem SwordTrail;
		public AudioSource audioSource;
        public GameObject PrefabAttack;
		public GameObject PrefabDeath;

        [Header("�Q�[���p�b�h�̃o�C�u���[�V�����ݒ�")]
		[Tooltip("�G���߂Â����Ƃ�")]
		public bool NearEnemy = true;
        [Tooltip("�G�ɋ߂��Ƃ��G�������Ă�����G�̑�������������")] 
        public bool EnemyFootstep = false;
        [Tooltip("�U�����m�̋����ݒ��AudioSoruce�̐ݒ�ɍ��킹��")] 
        public bool VibrationAudioDependency = true;

        [Tooltip("�U�����n�܂鋗��")]
        public float MinDistance = 0.0f;
        [Tooltip("�U�����n�܂鋗��")]
        public float MaxDistance = 15.0f;

		[Header("�����ݒ肳�����")]
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

		//�G���
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

			//�I�[�f�B�I�̐ݒ������
            if (VibrationAudioDependency)
            {
                MaxDistance = audioSource.maxDistance;
                MinDistance = audioSource.minDistance;
            }

			//�Q�[���p�b�h�f�o�C�X�擾
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
			//�W�����v���͔��肵�Ȃ�
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
                // �����N�[���^�C�����Z�b�g
				_fallTimeoutDelta = playerProperty.FallTimeout;

                // �㉺�̉����x���������Ȑ��l�ɂȂ�Ȃ��悤�ɗ}������
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

                // �W�����v
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// H * -2 * G �̕����� = �]�݂̍����ɓ��B���邽�߂ɕK�v�ȑ��x
					_verticalVelocity = Mathf.Sqrt(playerProperty.JumpHeight * -2f * playerProperty.Gravity);
                    _isJump = true;
                    Grounded = false;
                }

				// �W�����v�N�[���^�C��
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
                }
            }
			else
			{
				// �W�����v�N�[���^�C�����Z�b�g
				_jumpTimeoutDelta = playerProperty.JumpTimeout;

				// �����N�[���^�C��
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				//�㏸���ɓV��ɓ��������ꍇ������������
                if (_controller.velocity.y == 0.0f)
                {
                    _isJump = false;
                    _verticalVelocity = 0.0f;
                }

				//�W�����v���͎󂯕t����悤�ɂ���
                _input.jump = false;
			}


            // �d�͂ŗ��Ƃ�
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
            //�p�b�h�����������牽�����Ȃ�
			if (gamepad == null)
				return;

			//�G�����Ȃ��ꍇ�U�����I�t�ɂ��ďI���
            if (enemy == null)
            {
                gamepad.SetMotorSpeeds(0.0f, 0.0f);
                return;
			}

			//�G�ɋ߂Â��Ă�Ƃ�
            if (NearEnemy)
            {
				NearEnemyProcess();
            }

			//�G�ɋ߂Â��ĂĂ��G�������Ă�Ƃ�
            if (EnemyFootstep)
            {
				EnemyFootstepProcess();
            }
        }

        private void NearEnemyProcess()
        {
			//�����ŐU��
			DistanceVibration();
		}

        private void EnemyFootstepProcess()
        {
			//�����Ă���U��
            Vector2 vec = new Vector2(enemyCharacterController.velocity.x, enemyCharacterController.velocity.z);
            if (vec.magnitude < 0.1f)
            {
                gamepad.SetMotorSpeeds(0.0f, 0.0f);
                return;
			}

			//�U��
            DistanceVibration();
        }

        private void DistanceVibration()
        {
			//�����ɍ��킹�ĐU��������
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