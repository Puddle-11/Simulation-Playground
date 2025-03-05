using UnityEngine;
using UnityEngine.UI;

public class SpeachBars : MonoBehaviour
{
    [Header("Agression Settings")]
    [Header("-=-=-=-=-=-=-")]
    [Space]

    [Range(0, 1)]
    [SerializeField] private float agression;
    [SerializeField] private Vector2 magMinMax = new Vector2(10, 50);
    [SerializeField] private AnimationCurve magnitudeOverAgression = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private Vector2 freqMinMax = new Vector2(100, 70);
    [SerializeField] private AnimationCurve FrequencyOverAgression = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private Vector2 speedMinMax = new Vector2(80, 130);
    [SerializeField] private AnimationCurve speedOverAgression = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private Vector2 perlinSpeedMinMax = new Vector2(-80, -100);
    [SerializeField] private AnimationCurve perlinSpeedOverAgression = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private Vector2 perlinScaleMinMax = new Vector2(300, 100);
    [SerializeField] private AnimationCurve perlinScaleOverAgression = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private Gradient colorOverAgression;

    [Header("General Settings")]
    [Header("-=-=-=-=-=-=-")]
    [Space]
    [SerializeField] private AlignType Alignment;
    [SerializeField] private DisplayType displayType;
    [SerializeField] private bool checkAgression;
    [SerializeField] private int barCount = 30;
    [SerializeField] private float barDistance = 5;
    [SerializeField] private float barWidth = 15;

    [Range(0, 1)]
    [SerializeField] private float minBarScale = 0.4f;
    [Range(-1, 1)]
    [SerializeField] private float sinSpeedSync = 1;
    [Range(0, 1)]
    [SerializeField] private float perlinFactor  =0.8f;

    [SerializeField] private AnimationCurve perlinFalloff = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve sinRemap = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float magFlickerSpeedUp = 200;
    [SerializeField] private float magFlickerSpeedDown = 70;

    [SerializeField] private Vector2 magFlickerStrength = new Vector2(5, 10);
    [SerializeField] private Vector2 magFlickerInterval = new Vector2(0.1f, 1);
    [SerializeField] private float maxFlickerHeight = 30;


    private float perlinSpeed;
    private float sinFrequency;
    private float sinSpeed;
    private float perlinScale;
    private float magFlickerTimer;
    private float sinMagnitude; //base

    private float targetMagnitude; //target
    private float filteredSinMag; //usable
    private float sinTimer = 0;
    private float perlinTimer = 0;
    private RectTransform[] bars;
    public enum AlignType
    {
        Center,
        Left,
        Right
    }
    public enum DisplayType
    {
        Middle,
        Flat
    }
    public void Start()
    {
        GenerateBars();
        UpdateAgressionValues(agression);
    }
    public void Update()
    {

        if(checkAgression)  UpdateAgressionValues(agression);

        sinTimer += Time.deltaTime * sinSpeed;
        perlinTimer += Time.deltaTime * perlinSpeed;
        UpdateBars();

        if(Mathf.Abs(filteredSinMag - targetMagnitude) > 0.01f) filteredSinMag = Mathf.MoveTowards(filteredSinMag, targetMagnitude, magFlickerSpeedUp * Time.deltaTime);
        if (Mathf.Abs(sinMagnitude - targetMagnitude) > 0.01f) targetMagnitude = Mathf.MoveTowards(targetMagnitude, sinMagnitude, magFlickerSpeedDown * Time.deltaTime);

        magFlickerTimer -= Time.deltaTime;

        if (magFlickerTimer <= 0)
        {
            FlickerMag();
            magFlickerTimer = Random.Range(magFlickerInterval.x, magFlickerInterval.y);
        }
    }
    private void FlickerMag()
    {
        targetMagnitude += Random.Range(magFlickerStrength.x, magFlickerStrength.y) * 2;
        if (targetMagnitude > sinMagnitude + maxFlickerHeight)
        {
            targetMagnitude = sinMagnitude;
        }
    }
    private void GenerateBars()
    {
        bars = new RectTransform[barCount];
        for (int i = 0; i < barCount; i++)
        {
            GameObject g = new GameObject();
            g.transform.parent = transform;
            g.AddComponent<CanvasRenderer>();
            g.AddComponent<Image>();

            bars[i] = g.GetComponent<RectTransform>();
            bars[i].sizeDelta = new Vector2(barWidth, barWidth);
            switch (Alignment)
            {
                case AlignType.Center:
                    bars[i].anchoredPosition = new Vector2((barWidth + barDistance) * (i - barCount / 2.0f), 0);

                    break;
                case AlignType.Left:
                    bars[i].anchoredPosition = new Vector2((barWidth + barDistance) * i, 0);

                    break;
                case AlignType.Right:
                    bars[i].anchoredPosition = new Vector2((barWidth + barDistance) * -i, 0);
                    break;
            }
        }
    }
    private void UpdateBars()
    {
        float w;

        float perlinM;
        for (int i = 0; i < barCount; i++)
        {

            #region Sum Samples
            float nSum =
                SamplePerlin
                (
                    i * 15 + sinTimer + sinFrequency,
                    sinRemap,
                    filteredSinMag,
                    sinFrequency
                    )
                +
                SamplePerlin
                (
                    i * 15 - sinTimer * sinSpeedSync + sinFrequency + 150,
                    sinRemap,
                    filteredSinMag,
                    sinFrequency
                );
            #endregion

            w = barWidth * nSum / 2;

            #region Sum Perlin Samples
            //sum two perlin noise gradients moving in oposite directions
            perlinM =
                SamplePerlin
                (
                    bars[i].anchoredPosition.x - perlinTimer,
                    perlinFalloff,
                    1,
                    perlinScale
                )
                +
                SamplePerlin
                (
                    bars[i].anchoredPosition.x + perlinTimer,
                    perlinFalloff,
                    1,
                    perlinScale
                );
            #endregion

            //remap perlin after sum and applies factor
            perlinM /= 2 * perlinFactor + (1 - perlinFactor);

            w *= perlinM; //apply masking factor (so not all noise is displayed at the same volume but some areas are dampened
            w = Mathf.Max(w, barWidth * minBarScale); //reclamp (just in case)

            if (displayType == DisplayType.Flat)
            {
                bars[i].sizeDelta = new Vector2(barWidth, w/2);
                bars[i].anchoredPosition = new Vector2(bars[i].anchoredPosition.x, w / 4);
            }
            else
            {
                bars[i].sizeDelta = new Vector2(barWidth, w);
                bars[i].anchoredPosition = new Vector2(bars[i].anchoredPosition.x, 0);
            }
        }

    }
    #region Samples
    private float SamplePerlin(float _pos, AnimationCurve reMapCurve, float _mag = 1, float _scale = 1)
    {
        //less variables means faster so dont get on my ass for using oneliners :p
        return reMapCurve.Evaluate(Mathf.PerlinNoise((_pos + 0.01f) / _scale, (_pos + 0.01f) / _scale)) * _mag;
    }
    #endregion

    #region Getters and Setters
    public void SetAgression(float _val)
    {
        agression = _val;
        UpdateAgressionValues(agression);
    }
    public float GetAgression() { return agression; }
    #endregion

    #region Evaluate agression variables
    private void UpdateAgressionValues(float _ag)
    {
        Color c = colorOverAgression.Evaluate(_ag);
        sinMagnitude = magnitudeOverAgression.Evaluate(_ag) * (magMinMax.y - magMinMax.x) + magMinMax.x;
        sinFrequency = FrequencyOverAgression.Evaluate(_ag) * (freqMinMax.y - freqMinMax.x) + freqMinMax.x;
        sinSpeed = speedOverAgression.Evaluate(_ag) * (speedMinMax.y - speedMinMax.x) + speedMinMax.x;

        perlinScale = perlinScaleOverAgression.Evaluate(_ag) * (perlinScaleMinMax.y - perlinScaleMinMax.x) + perlinScaleMinMax.x;
        perlinSpeed = perlinSpeedOverAgression.Evaluate(_ag) * (perlinSpeedMinMax.y - perlinSpeedMinMax.x) + perlinSpeedMinMax.x;

        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].gameObject.GetComponent<Image>().color = c;
        }
    }
    #endregion
}
