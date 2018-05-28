using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class p_CameraController : p_Base 
{
    [SerializeField] Vector2 sensitivity;
    [SerializeField] float bodyCollMaxYOffset = -0.05f;
    [SerializeField] Vector3 offset;
    [SerializeField] float yNegativeRotLimit;
    [SerializeField] float yPositiveRotLimit;
    [SerializeField] float lockLeftOffset = 2f;
    [SerializeField] float lockLerpSpeed = 0.2f;
    [SerializeField] float fovLerpSpeed = 0.2f;
    [SerializeField] float normalFOV = 70;
    [SerializeField] float lockFOV = 65;
    Vector3 eulerRot = Vector3.zero;

    float curFov = 0f;

    private void Start()
    {
        eulerRot.y = CamPivot.localEulerAngles.y;
        curFov = Cam.fieldOfView;
        offset = CamPivot.localPosition;
    }

    protected void Update ()
    {
        if (EnemyLocked == null)
        {
            RotationHandler();

            if (InteractionState == p_InteractionController.e_InteractionState.Interacting)
                InteractingRotationHandler();

            ApplyRots();
        }
        else
        {
            ChangeLockedEnemyHandler();
            LockedEnemyRotationHandler();
        }

        FovHandler();
    }

    protected void LateUpdate()
    {
        CamPivot.position = CamPivot.TransformVector(offset) + Visual.position;
    }

    Vector2 viewInput;

    protected void RotationHandler()
    {
        viewInput = C.ViewInput;
        eulerRot.x = Utilities.Limit(eulerRot.x - viewInput.y * sensitivity.y, yNegativeRotLimit, yPositiveRotLimit, false);
        eulerRot.y = Utilities.Limit(eulerRot.y + viewInput.x * sensitivity.x, 0f, 360f, true);
    }

    public void ModEulerRotY(float y)
    {
        eulerRot.y += y;
    }

    protected void ChangeLockedEnemyHandler()
    {
        if (C.LockTargetRight)
            ChangeEnemyLocked(true);
        else if (C.LockTargetLeft)
            ChangeEnemyLocked(false);
    }
    protected void LockedEnemyRotationHandler()
    {
        Vector3 targetPoint = EnemyLocked.Body.position;
        Quaternion oRot = CamPivot.rotation;

        CamPivot.LookAt(targetPoint.SetY(CamPivot.position.y) + Visual.right * -lockLeftOffset);

        CamPivot.rotation = Quaternion.Lerp(oRot, CamPivot.rotation, lockLerpSpeed * Utilities.Delta);

        oRot = CamSecondPivot.rotation;
        CamSecondPivot.LookAt(targetPoint + Visual.right * -lockLeftOffset + EnemyLocked.transform.up * 1f);

        CamSecondPivot.rotation = Quaternion.Lerp(oRot, CamSecondPivot.rotation, lockLerpSpeed * Utilities.Delta);
        CamSecondPivot.localEulerAngles = CamSecondPivot.localEulerAngles.SetY(0).SetZ(0);

        float camLocalEulerX = CamSecondPivot.localEulerAngles.x;
        if (camLocalEulerX > 180)
            camLocalEulerX -= 360;

        eulerRot.x = Utilities.Limit(camLocalEulerX, yNegativeRotLimit, yPositiveRotLimit, false);
        eulerRot.y = Utilities.Limit(CamPivot.localEulerAngles.y, 0f, 360f, true);

    }

    protected void InteractingRotationHandler()
    {
        eulerRot.x = Quaternion.LookRotation(IC.CurrentInteractable.transform.position - CamSecondPivot.position).eulerAngles.x;
    }

    protected void FovHandler()
    {
        if (EnemyLocked == null)
            curFov = Utilities.Lerp(curFov, normalFOV, fovLerpSpeed);
        else
            curFov = Utilities.Lerp(curFov, lockFOV, fovLerpSpeed);

        Cam.fieldOfView = curFov + PostProcess.fovOffset;
    }

    void ApplyRots()
    {
        CamPivot.localEulerAngles = CamPivot.localEulerAngles.SetY(eulerRot.y);
        CamSecondPivot.localEulerAngles = CamSecondPivot.localEulerAngles.SetX(eulerRot.x);
        Cam_t.eulerAngles = Vector3.zero;
        Cam_t.LookAt(Cam_t.position + CamSecondPivot.forward);
    }

    public e_EnemyAI[] GetEnemiesOnScreen(float maxRange, LayerMask whatIsEnemy)
    {
        List<Collider> colls = new List<Collider>(Physics.OverlapSphere(Body.position, maxRange, whatIsEnemy));
        Vector2 screenPos;
        for (int i = colls.Count - 1; i >= 0; i--)
        {
            if (Cam_t.InverseTransformVector(colls[i].transform.position - Cam_t.position).z < 0 || Physics.Raycast(Cam_t.position, (colls[i].transform.position - Cam_t.position), (colls[i].transform.position - Cam_t.position).magnitude, Game_NG.GroundLayerMask))
            {
                colls.Remove(colls[i]);
            }
            else
            {
                screenPos = Cam.WorldToScreenPoint(colls[i].transform.position);
                if (screenPos.x < -20f || screenPos.x > Screen.width + 20f)
                    colls.Remove(colls[i]);
                else if (screenPos.y < -20f || screenPos.y > Screen.height + 20f)
                    colls.Remove(colls[i]);
            }
        }
        List<e_EnemyAI> enemies = new List<e_EnemyAI>();

        for (int i = 0; i < colls.Count; i++)
            enemies.Add(colls[i].GetComponentInParent<e_EnemyAI>());

        return enemies.ToArray();
    }

    protected void ChangeEnemyLocked(bool right)
    {
        List<e_EnemyAI> enemies = new List<e_EnemyAI>(GetEnemiesOnScreen(LockRange, AC.whatIsEnemy));
        List<e_EnemyAI> sortedEnemies = new List<e_EnemyAI>();

        while (enemies.Count > 0)
        {
            float distFromleft = Mathf.Infinity;
            e_EnemyAI nearest = null;
            for (int i = 0; i < enemies.Count; i++)
            {
                float dist;
                if ((dist = Cam.WorldToScreenPoint(enemies[i].Body.position).x) < distFromleft)
                {
                    nearest = enemies[i];
                    distFromleft = dist;
                }
            }
            sortedEnemies.Add(nearest);
            enemies.Remove(nearest);
        }

        int index = sortedEnemies.IndexOf(EnemyLocked);
        index = (int)Utilities.AddWithLimit(index, right ? 1 : -1, 0, sortedEnemies.Count - 1, false);
        EnemyLocked = sortedEnemies[index];
    }


    
}