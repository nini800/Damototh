using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utilities : MonoBehaviour
{

    #region Static Part
    public static Utilities Instance;
    public static float Delta { get { return Utilities.Limit(Time.deltaTime * Time.timeScale, null, 1 / 24f) * 60f; } }

    /// <summary>
    /// Return float with added amount and caped to specified limits.
    /// </summary>
    /// <param name="whileIfLimit"></param>
    /// If it goes up or down a limit, it will while. 1 - 10 : if we have 12 it will results in 2.
    /// <returns></returns>
    public static float AddWithLimit(float number, float amount, float lowerLimit, float upperLimit, bool whileIfLimit = false)
    {
        number += amount;

        if (number < lowerLimit)
        {
            if (whileIfLimit)
            {
                number -= lowerLimit;
                number += upperLimit;
            }
            else
                number = lowerLimit;
        }
        else if (number > upperLimit)
        {
            if (whileIfLimit)
            {
                number -= upperLimit;
                number += lowerLimit;
            }
            else
                number = upperLimit;

        }

        return number;
    }

    /// <summary>
    /// Substract the given amount with params.
    /// </summary>
    /// /// <param name="limit"></param>
    /// if absolute = false, can get under that limit, if absoulte = true, both under and upper values will
    /// try to reach the limit and stick to it
    /// /// <param name="absolute"></param>
    /// if absolute = false, can get under that limit, if absoulte = true, both under and upper values will
    /// try to reach the limit and stick to it
    /// <returns></returns>
    public static float SubtractFloat(float number, float amount, float limit = 0f, bool absolute = true)
    {
        if (!absolute)
        {
            number -= amount;
            if (number < limit)
                number = limit;
            return number;
        }
        else
        {
            bool negative = number < 0;
            number -= negative ? -amount : amount;

            if (negative && number > limit)
                number = limit;
            else if (!negative && number < limit)
                number = limit;

            return number;
        }
    }
    public static Vector3 Limit(Vector3 v, float? lowerLimit = null, float? upperLimit = null)
    {
        if (lowerLimit != null && v.magnitude < lowerLimit)
            return v.normalized * (float)lowerLimit;
        if (upperLimit != null && v.magnitude > upperLimit)
            return v.normalized * (float)upperLimit;
        return v;
    }

    public static float GetTrigDistance(float a, float b)
    {
        float final = Mathf.Abs(a - b);
        if (Mathf.Abs((a - 360) - b) < final)
            final = Mathf.Abs((a - 360) - b);

        if (Mathf.Abs((a + 360) - b) < final)
            final = Mathf.Abs((a + 360) - b);

        return final;
    }


    public static float Limit(float number, float? lowerLimit = null, float? upperLimit = null, bool whileOnExcess = false)
    {
        if (!whileOnExcess)
        {
            if (lowerLimit != null && number < lowerLimit)
                return (float)lowerLimit;
            if (upperLimit != null && number > upperLimit)
                return (float)upperLimit;
        }
        else
        {
            if (lowerLimit != null && number < lowerLimit)
                return (float)(upperLimit - (lowerLimit - number));
            if (upperLimit != null && number > upperLimit)
                return (float)(lowerLimit + (number - upperLimit));
        }

        return number;
    }
    public static int Limit(int number, int? lowerLimit = null, int? upperLimit = null)
    {
        if (lowerLimit != null && number < lowerLimit)
            return (int)lowerLimit;
        if (upperLimit != null && number > upperLimit)
            return (int)upperLimit;
        return number;
    }
    public static float Lerp(float number, float target, float t, bool trigoMode = false)
    {
        if (!trigoMode)
            return (number * (1 - t) + target * t);
        else
        {
            if (Mathf.Abs(target - (number - 360)) < Mathf.Abs(target - number))
            {
                return ((number - 360) * (1 - t) + target * t);
            }
            else if (Mathf.Abs(target - (number + 360)) < Mathf.Abs(target - number))
            {
                return ((number + 360) * (1 - t) + target * t);
            }

            return (number * (1 - t) + target * t);
        }
    }
    public static float ChangeToward(float number, float target, float amount, bool trigoMode = false)
    {
        if (number == target)
            return number;
        amount = Mathf.Abs(amount);
        if (!trigoMode)
            return Limit(number + (target < number ? -amount : amount), target < number ? (float?)target : null, target < number ? null : (float?)target);
        else
        {
            float absDifference = Mathf.Abs(number - target);
            float trigDifference = target - number;
            float futureOffset = 0f;
            if (Mathf.Abs(number + 360 - target) < absDifference)
            {
                absDifference = Mathf.Abs(target - (number + 360));
                trigDifference = target - (number + 360);
                futureOffset = 360;
            }
            if (Mathf.Abs(number - 360 - target) < absDifference)
            {
                absDifference = Mathf.Abs(target - (number - 360));
                trigDifference = target - (number - 360);
                futureOffset = -360f;
            }
            number += futureOffset;

            return Limit(number + (trigDifference < 0 ? -amount : amount), target < number ? (float?)target : null, target < number ? null : (float?)target);
        }
    }
    public static float Linearity(float number, float min, float max, float pow, bool scaleToOne = false, bool revert = false)
    {
        if (pow < 0)
            Debug.LogError("Pow must be positive");

        number -= min;
        number /= (max - min);
        number = Mathf.Pow(number, pow);


        if (!revert)
        {
            if (!scaleToOne)
                return (number * (max - min) + min);
            else
                return (number);
        }
        else
        {
            if (!scaleToOne)
                return ((max - min) - (number * (max - min))) + min;
            else
                return 1 - number;
        }

    }
    public enum CalculMode
    {
        ThreeAxis,
        NoYAxis
    }
    public enum LimitMode
    {
        Set,
        Smooth
    }
    /// <summary>
    /// Add magnitude to v's magnitude. If v's magnitude > upperlimit, it is set to upperlimit.
    /// </summary>
    public static Vector3 AddWithLimit(Vector3 v, float magnitude, float upperLimit, LimitMode limitMode = LimitMode.Set)
    {
        if (v.magnitude + magnitude < upperLimit)
            return v.normalized * (v.magnitude + magnitude);
        else
        {
            switch (limitMode)
            {
                case LimitMode.Set:
                    return v.normalized * upperLimit;
                case LimitMode.Smooth:
                    if (SubtractMagnitude(v, magnitude).magnitude < upperLimit)
                        return v.normalized * upperLimit;
                    else
                        return SubtractMagnitude(v, magnitude);
                default:
                    return v.normalized * upperLimit;
            }
        }

    }
    /// <summary>
    /// Add add to v. If v's magnitude > upperlimit, it is set to upperlimit.
    /// </summary>
    public static Vector3 AddWithLimit(Vector3 v, Vector3 add, float upperLimit, CalculMode mode = CalculMode.ThreeAxis, LimitMode limitMode = LimitMode.Set, float? smoothLimitSubstract = null)
    {
        switch (mode)
        {
            case CalculMode.ThreeAxis:
                v += add;
                if (v.magnitude < upperLimit)
                    return v;
                else
                {
                    switch (limitMode)
                    {
                        case LimitMode.Set:
                            return v.normalized * upperLimit;
                        case LimitMode.Smooth:
                            if (SubtractMagnitude(v, (smoothLimitSubstract == null ? add.magnitude * 2f : (float)smoothLimitSubstract + add.magnitude)).magnitude < upperLimit)
                                return v.normalized * upperLimit;
                            else
                                return SubtractMagnitude(v, (smoothLimitSubstract == null ? add.magnitude * 2f : (float)smoothLimitSubstract + add.magnitude));
                        default:
                            return v.normalized * upperLimit;
                    }
                }


            case CalculMode.NoYAxis:
                float y = v.y;
                v.y = 0;
                add.y = 0;
                v += add;
                if (v.magnitude < upperLimit)
                    return (new Vector3(v.x, y, v.z));
                else
                {
                    Vector3 temp = v.normalized * upperLimit;

                    switch (limitMode)
                    {
                        case LimitMode.Set:
                            return new Vector3(temp.x, y, temp.z).normalized * upperLimit;
                        case LimitMode.Smooth:
                            if (SubtractMagnitude(v, (smoothLimitSubstract == null ? add.magnitude * 2f : (float)smoothLimitSubstract + add.magnitude)).magnitude < upperLimit)
                                return new Vector3(temp.x, y, temp.z);
                            else
                            {
                                v = SubtractMagnitude(v, (smoothLimitSubstract == null ? add.magnitude * 2f : (float)smoothLimitSubstract + add.magnitude));
                                return new Vector3(v.x, y, v.z);
                            }
                        default:
                            return new Vector3(v.x, y, v.z).normalized * upperLimit;
                    }
                }

            default:
                return Vector3.zero;
        }

    }
    /// <summary>
    /// Add add to v. If v's magnitude > upperlimit, it is set to upperlimit.
    /// </summary>
    public static Vector2 AddWithLimit(Vector2 v, Vector2 add, float upperLimit, LimitMode limitMode = LimitMode.Set)
    {
        v += add;

        if (v.magnitude < upperLimit)
            return v;
        else
        {
            switch (limitMode)
            {
                case LimitMode.Set:
                    return v.normalized * upperLimit;
                case LimitMode.Smooth:
                    if (SubstractMagnitude(v, add.magnitude).magnitude < upperLimit)
                        return v.normalized * upperLimit;
                    else
                        return SubstractMagnitude(v, add.magnitude);
                default:
                    return v.normalized * upperLimit;
            }
        }
    }

    /// <summary>
    /// Substract magnitude to v's magnitude. return vector.zero if magnitude > v.magnitude
    /// </summary>
    public static Vector3 SubtractMagnitude(Vector3 v, float magnitude, CalculMode mode = CalculMode.ThreeAxis)
    {
        switch (mode)
        {
            case CalculMode.ThreeAxis:
                if (v.magnitude > magnitude)
                    return v.normalized * (v.magnitude - magnitude);
                else
                    return Vector3.zero;
            case CalculMode.NoYAxis:
                float y = v.y;
                v.y = 0f;
                if (v.magnitude > magnitude)
                {
                    v = v.normalized * (v.magnitude - magnitude);
                    return new Vector3(v.x, y, v.z);
                }
                else
                    return new Vector3(0f, y, 0f);
            default:
                return Vector3.zero;
        }

    }

    /// <summary>
    /// Substract magnitude to v's magnitude. return vector.zero if magnitude > v.magnitude
    /// </summary>
    public static Vector2 SubstractMagnitude(Vector2 v, float magnitude)
    {
        if (v.magnitude > magnitude)
            return v.normalized * (v.magnitude - magnitude);
        else
            return Vector3.zero;

    }

    public static Vector2 RandomPointInCircle(float radius)
    {
        float randAngle = Random.Range(0f, Mathf.PI * 2);
        float randDist = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;

        return new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle)) * randDist;
    }

    /// <summary>
    /// Convert Vector 3 to 2. The Z axis is lost.
    /// </summary>
    public static Vector2 Vector3To2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }
    /// <summary>
    /// Convert Vector 2 to 3. The Z axis can be specified.
    /// </summary>
    public static Vector3 Vector2To3(Vector2 v, float z = 0f)
    {
        return new Vector3(v.x, v.y, z);
    }
    public enum AxisCouple
    {
        XandY,
        XandZ,
        YandZ
    }
    /// <summary>
    ///  Return the same vector with two axis swapped.
    /// </summary>
    public static Vector3 SwapAxis(Vector3 v, AxisCouple axis)
    {
        switch (axis)
        {
            case AxisCouple.XandY:
                return new Vector3(v.y, v.x, v.z);
            case AxisCouple.XandZ:
                return new Vector3(v.z, v.y, v.x);
            case AxisCouple.YandZ:
                return new Vector3(v.x, v.z, v.y);
            default:
                return Vector3.zero;
        }

    }

    public static Vector3 RemoveYAxis(Vector3 v)
    {
        return new Vector3(v.x, 0f, v.z);
    }

    public static float GetAngle(Vector2 a)
    {
        a.Normalize();

        if (a.y > 0)
            return -Mathf.Acos(a.x) * Mathf.Rad2Deg;
        else
            return Mathf.Acos(a.x) * Mathf.Rad2Deg;
    }

    public static Vector2 DeadzoneVector(Vector2 vec, float innerDeadzone = 0.1f, float outterDeadzone = 0f, float linearity = 1f)
    {
        if (vec.magnitude < innerDeadzone)
        {
            return Vector2.zero;
        }
        else if (vec.magnitude > 1 - outterDeadzone)
        {
            return vec.normalized;
        }
        else
        {
            vec = SubstractMagnitude(vec, innerDeadzone);
            vec *= 1 / (1 - outterDeadzone - innerDeadzone);
            float x = Mathf.Pow(Mathf.Abs(vec.x), linearity);
            float y = Mathf.Pow(Mathf.Abs(vec.y), linearity);
            //Debug.Log(x + " " + Mathf.Pow(vec.x, linearity) + " " + vec.x);
            x *= vec.x < 0 ? -1 : 1;
            y *= vec.y < 0 ? -1 : 1;
            return new Vector2(x, y);
        }
    }


    public static void SetLayerOfAllChildrens(Transform transform, LayerMask layer)
    {
        transform.gameObject.layer = layer;

        foreach (Transform t in transform)
        {
            SetLayerOfAllChildrens(t, layer);
        }
    }

    public static float GetColorMaxima(Color color)
    {
        float maxima = color.r;
        if (color.g > maxima)
            maxima = color.g;
        if (color.b > maxima)
            maxima = color.b;

        return maxima;
    }
    #endregion

    #region Instantiated Part

    public enum HideCursorType
    {
        Show,
        Hide,
        Lock,
        HideAndLock
    }

    [SerializeField] HideCursorType CursorOnStart = HideCursorType.HideAndLock;
    [SerializeField] KeyCode CursorKey = KeyCode.K;
    [SerializeField] GameObject[] DDOLObjects;
    [SerializeField] AudioSource sourceForMiddleClick;

    static bool firstTime = true;

    void Awake()
    {
        Instance = this;
        for (int i = 0; i < DDOLObjects.Length; i++)
        {
            if (firstTime)
                DontDestroyOnLoad(DDOLObjects[i]);
            else
                Destroy(DDOLObjects[i]);
        }
        firstTime = false;
    }

    void Start()
    {
        switch (CursorOnStart)
        {
            case HideCursorType.Hide:
                Cursor.visible = false;
                break;
            case HideCursorType.Lock:
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case HideCursorType.HideAndLock:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;
            default:
                break;
        }


    }

    void Update()
    {
        HideCursorHandler();

        if (Input.GetMouseButtonDown(2))
        {
            sourceForMiddleClick.Play(0);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            FindObjectOfType<p_PlayerBeing>().AddBlood(-10000f);
            FindObjectOfType<p_PlayerBeing>().AddMadness(-10000f);
            FindObjectOfType<p_PlayerBeing>().AddHealth(10000f);
        }

    }

    void HideCursorHandler()
    {
        if(Input.GetKeyDown(CursorKey))
        {
            switch (CursorOnStart)
            {
                case HideCursorType.Hide:
                    Cursor.visible = !Cursor.visible;
                    break;
                case HideCursorType.Lock:
                    Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
                    break;
                case HideCursorType.HideAndLock:
                    Cursor.visible = !Cursor.visible;
                    Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
                    break;
                default:
                    break;
            }
        }


    }
    #endregion

}

[System.Serializable]
public class TransformMovement
{
    public Transform transform;
    public Vector3 translation;
    public float t_Time;
    public float t_Linearity = 1;
    public Vector3 rotation;
    public float r_Time;
    public float r_Linearity = 1;
    public float timeBeforeStart;
    public AudioSource soundSource;
    public TransformMovementType type;
    public bool canPlayMultipleTimes = true;
    public bool canParentPlayer = false;
    public bool canParentObjects = false;

    public float TotalTime
    {
        get
        {
            float maxTime = 0f;
            if (r_Time > maxTime)
                maxTime = r_Time;
            if (t_Time > maxTime)
                maxTime = t_Time;

            return maxTime;
        }
    }

    bool alreadyPlayed = false;

    public MonoTransformMovement mono { get; set; }

    public void InitiateMovement(bool reversed = false)
    {
        if (alreadyPlayed && !canPlayMultipleTimes)
            return;

        alreadyPlayed = true;

        if (mono == null)
        {
            if (!(mono = transform.gameObject.GetComponent<MonoTransformMovement>()))
            {
                mono = transform.gameObject.AddComponent<MonoTransformMovement>();
            }
        }

        mono.StartCoroutine(InitiateWait(reversed));
    }
    IEnumerator InitiateWait(bool reversed)
    {
        yield return new WaitForSeconds(timeBeforeStart);
        WaitComplete(reversed);
    }
    void WaitComplete(bool reversed)
    {
        mono.movement = this;
        mono.StopMovementCoroutines();

        switch (type)
        {
            case TransformMovementType.OneShotCanNotRevert:

                if (translation.magnitude > 0)
                    mono.t_Coroutine = mono.StartCoroutine("OneShotMovementTranslation", reversed);
                if (rotation.magnitude > 0)
                    mono.r_Coroutine = mono.StartCoroutine("OneShotMovementRotation", reversed);
                break;
        }


        if (soundSource != null)
            soundSource.Play(0);
    }
}

public class MonoTransformMovement : MonoBehaviour
{
    public TransformMovement movement;
    
    public Vector3 t_Target, r_Target;
    public Vector3 t_Origin, r_Origin;
    public Coroutine t_Coroutine, r_Coroutine;

    public IEnumerator OneShotMovementTranslation(bool reversed)
    {
        t_Origin = transform.position;
        t_Target = t_Origin + (!reversed ? movement.translation : -movement.translation);

        float counter = 0f;
        while (counter < movement.t_Time)
        {
            transform.position = Vector3.Lerp(t_Origin, t_Target, Utilities.Linearity(counter / movement.t_Time, 0, 1, movement.t_Linearity, true));

            yield return new WaitForEndOfFrame();
            counter += Time.deltaTime;
        }

        transform.position = t_Target;
    }

    public IEnumerator OneShotMovementRotation(bool reversed)
    {
        r_Origin = transform.eulerAngles;
        r_Target = r_Origin + (!reversed ? movement.rotation : -movement.rotation);


        float counter = 0f;
        while (counter < movement.r_Time)
        {
            transform.localRotation = Quaternion.Lerp(Quaternion.Euler(r_Origin), Quaternion.Euler(r_Target), Utilities.Linearity(counter / movement.r_Time, 0, 1, movement.r_Linearity, true));

            yield return new WaitForEndOfFrame();
            counter += Time.deltaTime;
        }

        transform.localRotation = Quaternion.Euler(r_Target);
    }

    public void StopMovementCoroutines()
    {

        if (t_Coroutine != null)
        {
            transform.position = t_Target;
            StopCoroutine(t_Coroutine);
            t_Coroutine = null;
        }
        if (r_Coroutine != null)
        {
            transform.localEulerAngles = r_Target;
            StopCoroutine(r_Coroutine);
            r_Coroutine = null;
        }
        
    }


    Transform player;
    void OnCollisionEnter(Collision coll)
    {
        if (movement.canParentPlayer &&coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            player = coll.gameObject.GetComponentInParent<Rigidbody>().transform.parent;
            player.GetComponentInChildren<Rigidbody>().interpolation = RigidbodyInterpolation.None;
            player.SetParent(transform, true);
        }
        else if (movement.canParentObjects && !(coll.gameObject.layer == LayerMask.NameToLayer("Player")))
        {
            coll.transform.SetParent(transform);
        }
    }
    void OnCollisionExit(Collision coll)
    {
        if (movement.canParentPlayer && coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            player.SetParent(Utilities.Instance.transform.parent, true);
            player.GetComponentInChildren<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
            player = null;

        }
        else if (movement.canParentObjects && !(coll.gameObject.layer == LayerMask.NameToLayer("Player")))
        {
            coll.transform.SetParent(Utilities.Instance.transform.parent);
        }
    }
}
public enum TransformMovementType
{
    OneShotCanNotRevert
}