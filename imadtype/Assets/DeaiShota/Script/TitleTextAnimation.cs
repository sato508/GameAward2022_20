/*------------------------------------------------------
 * 
 *  [TitleTextAnimation.cs]
 *  Author : 出合翔太
 *  タイトルシーンのアニメーションを管理
 * 
 ------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TitleTextAnimation : MonoBehaviour
{
    /// <summary>
    /// TextMeshPro TextComponent
    /// </summary>
    private TMP_Text _Text;

    /// <summary>
    /// ループするかどうか
    /// </summary>
    [SerializeField] private bool _IsLoop = false;

    /// <summary>
    /// グラデーションカラー
    /// </summary>
    [SerializeField] private Gradient _GradientColor;

    /// <summary>
    /// アニメーションの最大時間
    /// </summary>
    [SerializeField] private float _MaxAnimTime = 1.0f;

    /// <summary>
    /// アニメーションの時間
    /// </summary>
    private float _Time = 0.0f;

    /// <summary>
    /// 再生フラグ
    /// </summary>
    private bool _IsPlaying = false;

    /// <summary>
    /// アニメーション文字範囲(C#)
    /// </summary>
    private RangeInt _CharaIndexRange = new RangeInt(0, 0);
    /// <summary>
    /// アニメーション文字範囲(Shader)
    /// </summary>
    private RangeInt _PrimitiveIndexRange = new RangeInt(0, 0);

    private void Awake()
    {
        _Text = GetComponent<TMP_Text>();
        ResetMaterialAnimation();
    }

    private void OnDestroy()
    {
        ResetMaterialAnimation();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 時間計測
        if (_IsPlaying || _IsLoop)
        {
            _Time += Time.deltaTime;
            // ループする場合
            if (_IsLoop)
            {
                if (_Time >= _MaxAnimTime)
                {
                    _Time -= _MaxAnimTime;
                }
            }
            // ループしない場合
            else
            {
                if (_Time >= _MaxAnimTime)
                {
                    _Time = _MaxAnimTime;
                    _IsPlaying = false;
                    ResetMaterialAnimation();
                }
            }

            // アニメーションを更新
            UpdateAnimation(_Time / _MaxAnimTime);
        }
    }

    /// <summary>
    /// マテリアル（アニメーション値）のリセット
    /// </summary>
    private void ResetMaterialAnimation()
    {
        if (_Text != null)
        {
            TMP_TextInfo textInfo = _Text.textInfo;
            if (textInfo != null && 0 < textInfo.meshInfo.Length)
            {
                for (int i = 0; i < textInfo.materialCount; i++)
                {
                    if (textInfo.meshInfo[i].material == null)
                    {
                        continue;
                    }

                    textInfo.meshInfo[i].material.SetFloat("_AnimationTime", 0.0f);
                    textInfo.meshInfo[i].material.SetInt("_AnimationStartVertexID", 0);
                    textInfo.meshInfo[i].material.SetInt("_AnimationEndVertexID", 0);
                }
            }
        }
    }

    /// <summary>
    /// アニメーションの更新
    /// <param name = "Time"> 0.0f～1.0fのアニメーションの再生時間 </param>
    /// </summary>
    private void UpdateAnimation(float Time)
    {
        // ジオメトリ情報の初期化
        _Text.ForceMeshUpdate(true, true);
        TMP_TextInfo textInfo = _Text.textInfo;

        for (int i = 0; i < textInfo.characterInfo.Length; i++)
        {
            // 文字情報インデックスの範囲指定
            if (_CharaIndexRange.start <= i && i < _CharaIndexRange.end)
            {
                // 文字情報・メッシュ情報の取得
                var charaInfo = textInfo.characterInfo[i];
                if (!charaInfo.isVisible)
                {
                    continue;
                }

                int materialIndex = charaInfo.materialReferenceIndex;
                int vertexIndex = charaInfo.vertexIndex;
                var meshInfo = textInfo.meshInfo[materialIndex];
                // ④頂点情報の編集
                Vector3[] vertex = new Vector3[4];
                vertex[0] = meshInfo.vertices[vertexIndex + 0];
                vertex[1] = meshInfo.vertices[vertexIndex + 1];
                vertex[2] = meshInfo.vertices[vertexIndex + 2];
                vertex[3] = meshInfo.vertices[vertexIndex + 3];


                // 回転                
                float x = Mathf.PerlinNoise(i * 0.1f, 0.4f) * 1.0f;
                float y = Mathf.PerlinNoise(i * 0.2f, 0.5f) * 1.0f;
                float z = Mathf.PerlinNoise(i * 0.3f, 0.6f) * 1.0f;
                Vector3 rotationNoise = new Vector3(x, y, z);

                var center = Vector3.Scale(vertex[2] - vertex[0], Vector3.one * 0.5f) + vertex[0];
                var matrix = Matrix4x4.Rotate(Quaternion.Euler(rotationNoise * 360f * Time));
                vertex[0] = matrix.MultiplyPoint(vertex[0] - center) + center;
                vertex[1] = matrix.MultiplyPoint(vertex[1] - center) + center;
                vertex[2] = matrix.MultiplyPoint(vertex[2] - center) + center;
                vertex[3] = matrix.MultiplyPoint(vertex[3] - center) + center;

                // 移動
                x = Mathf.PerlinNoise(i * 0.7f, 0.1f) * 1.0f;
                y = Mathf.PerlinNoise(i * 0.8f, 0.2f) * 1.0f;
                z = Mathf.PerlinNoise(i * 0.9f, 0.3f) * 1.0f;
                Vector3 positionNoise = new Vector3(x, y, z);

                positionNoise = positionNoise * 3.0f * Time;

                Color color = _GradientColor.Evaluate(Time);
                for (int j = 0; j < vertex.Length; j++)
                {
                    vertex[j] += positionNoise;
                    // 代入
                    meshInfo.vertices[vertexIndex + j] = vertex[j];
                    // 色
                    meshInfo.colors32[vertexIndex + j] *= color;
                }
            }
        }

        // ジオメトリ情報の更新
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            if (textInfo.meshInfo[i].mesh == null)
            {
                continue;
            }

            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            _Text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);

            Material material = textInfo.meshInfo[i].material;
            material.SetFloat("_AnimationTime", Time);
            material.SetInt("_AnimationStartPrimitiveID", _PrimitiveIndexRange.start);
            material.SetInt("_AnimationEndPrimitiveID", _PrimitiveIndexRange.end);
        }
    }

    /// <summary>   
    /// Linkで囲われたテキストをクリックするとアニメーションの再生開始
    /// </summary>
    /// <param name="LinkID"> Linkのタグ名 </param>
    /// <param name="LinkText"> 囲われたテキスト </param>
    /// <param name="LinkTextFirstCharaIndex"> Linkタグに囲われた文字列の先頭文字要素 </param>
    /// <param name="LinkIndex"> </param>
    public void OnTouchLink(string LinkID, string LinkText, int LinkTextFirstCharaIndex, int LinkIndex = 0)
    {
        _IsPlaying = true;
        _Time = 0.0f;
        _CharaIndexRange.start = LinkTextFirstCharaIndex;
        _CharaIndexRange.length = LinkText.Length;

        int primitiveStart = LinkTextFirstCharaIndex;
        for (int i = LinkTextFirstCharaIndex - 1; 0 <= i; i--)
        {
            if (_Text.textInfo.characterInfo[i].isVisible == false)
            {
                primitiveStart--;
            }
        }

        int primitiveLength = 0;
        for (int i = _CharaIndexRange.start; i < _CharaIndexRange.end; i++)
        {
            if (_Text.textInfo.characterInfo[i].isVisible == true)
            {
                primitiveLength++;
            }
        }

        _PrimitiveIndexRange.start = primitiveStart * 2;
        _PrimitiveIndexRange.length = primitiveLength * 2;
    }
}
