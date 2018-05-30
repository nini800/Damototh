using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(p_AttackController))]
public class AttackControllerEditor : Editor 
{

    public override void OnInspectorGUI()
    {
        p_AttackController ac = (p_AttackController)target;
        EditorGUIUtility.labelWidth = 200;
        EditorGUILayout.LabelField("Attacks", EditorStyles.boldLabel); 

        ac.attackState = (p_AttackController.e_AttackState)EditorGUILayout.EnumPopup("Attack State", ac.attackState);
        EditorGUI.indentLevel++;
        if (ac.baseAttacks == null)
        {
            EditorUtility.SetDirty(ac);

            ac.baseAttacks = new PlayerAttackStats[0];
            Debug.Log("Creating base attacks");
        }
        DisplayAttackGroup(ref ac.attackFold, ref ac.baseAttacks, ac, "Base Attacks");
        EditorGUI.indentLevel--;

        EditorGUILayout.LabelField("Mutilate", EditorStyles.boldLabel);
        ac.mutilateCastTime = EditorGUILayout.FloatField("Cast Time", ac.mutilateCastTime);
        ac.mutilateHealthCost = EditorGUILayout.FloatField("Health Cost", ac.mutilateHealthCost);
        ac.mutilateBloodGain = EditorGUILayout.FloatField("Blood Gain", ac.mutilateBloodGain);
        ac.mutilateMadnessGain = EditorGUILayout.FloatField("Madness Gain", ac.mutilateMadnessGain);

        EditorGUILayout.LabelField("Lock", EditorStyles.boldLabel);
        ac.lockRange = EditorGUILayout.FloatField("Range", ac.lockRange);


        EditorUtility.SetDirty(ac);
    }

    void DisplayAttackGroup(ref bool fold, ref PlayerAttackStats[] atts, p_AttackController ac, string foldName)
    {
        EditorGUILayout.BeginHorizontal();
        fold = EditorGUILayout.Foldout(fold, foldName, true, EditorStyles.boldLabel);
        if (fold && GUILayout.Button("+", GUILayout.MaxWidth(50)))
        {
            List<PlayerAttackStats> list = new List<PlayerAttackStats>(atts);
            list.Add(new PlayerAttackStats());
            atts = list.ToArray();

            EditorUtility.SetDirty(ac);
        }
        EditorGUILayout.EndHorizontal();

        if (fold)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < atts.Length; i++)
            {
                DisplayAttack(atts[i], ac, ref atts);
            }
            EditorGUI.indentLevel--;
        }
    }

    void DisplayAttack(PlayerAttackStats att, p_AttackController ac, ref PlayerAttackStats[] parentRef)
    {
        EditorGUILayout.BeginHorizontal();

        att.fold = EditorGUILayout.Foldout(att.fold, att.Name, true, EditorStyles.boldLabel);

        if (GUILayout.Button("-", GUILayout.MaxWidth(40)))
        {
            List<PlayerAttackStats> list = new List<PlayerAttackStats>(parentRef);
            list.Remove(att);
            parentRef = list.ToArray();
            EditorUtility.SetDirty(ac);
            return;
        }
        EditorGUILayout.EndHorizontal();

        if (att.fold)
        {
            EditorGUI.indentLevel++;
            att.Name = EditorGUILayout.TextField("Name", att.Name);
            att.AttackInput = (PlayerAttackStats.AttackInputType)EditorGUILayout.EnumPopup("Attack Input", att.AttackInput);
            att.MoveInput = (p_MovementController.e_MovementState)EditorGUILayout.EnumFlagsField("Move Input", att.MoveInput);
            EditorGUILayout.Separator();
            att.damages = EditorGUILayout.FloatField("Damages", att.damages);
            att.knockback = EditorGUILayout.Vector3Field("Knockback", att.knockback);
            att.stunForce = EditorGUILayout.FloatField("Stun Force", att.stunForce);
            att.stunTime = EditorGUILayout.FloatField("Stun Time", att.stunTime);
            EditorGUILayout.Separator();
            att.followCaster = EditorGUILayout.Toggle("Follow Caster", att.followCaster);
            att.hitboxLifeTime = EditorGUILayout.FloatField("Hitbox Lifetime", att.hitboxLifeTime);
            att.maxHitTimes = EditorGUILayout.IntField("Max Hits", att.maxHitTimes);
            EditorGUILayout.Separator();
            att.CastTime = EditorGUILayout.FloatField("Cast Time", att.CastTime);
            att.AttackTime = EditorGUILayout.FloatField("Attack Time", att.AttackTime);
            att.RecoverTime = EditorGUILayout.FloatField("Recover Time", att.RecoverTime);
            EditorGUILayout.Separator();
            att.attackModel = (GameObject)EditorGUILayout.ObjectField("Attack Model", att.attackModel, typeof(GameObject), false);
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            att.impulseFold = EditorGUILayout.Foldout(att.impulseFold, "Impulses", true, EditorStyles.boldLabel);

            if (att.impulseFold && GUILayout.Button("+", GUILayout.MaxWidth(35)))
            {
                List<AttackStats.Impulse> list = new List<AttackStats.Impulse>(att.impulses);
                list.Add(new AttackStats.Impulse());
                EditorUtility.SetDirty(ac);
                att.impulses = list.ToArray();
            }
            EditorGUILayout.EndHorizontal();

            if (att.impulseFold)
            {
                for (int i = 0; i < att.impulses.Length; i++)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginHorizontal();
                    float totalTime = 0f;
                    for (int j = 0; j <= i; j++)
                        totalTime += att.impulses[j].time;

                    att.impulses[i].fold = EditorGUILayout.Foldout(att.impulses[i].fold, totalTime.ToString("0.00") + " : " + att.impulses[i].impulse.magnitude.ToString("0.00"), true, EditorStyles.boldLabel);

                    if (GUILayout.Button("-", GUILayout.MaxWidth(30)))
                    {
                        List<AttackStats.Impulse> list = new List<AttackStats.Impulse>(att.impulses);
                        list.Remove(att.impulses[i]);
                        att.impulses = list.ToArray();
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (att.impulses[i].fold)
                    {
                        EditorGUI.indentLevel++;
                        att.impulses[i].impulse = EditorGUILayout.Vector3Field("Impulse", att.impulses[i].impulse);
                        att.impulses[i].time = EditorGUILayout.FloatField("Time", att.impulses[i].time);
                        att.impulses[i].duration = EditorGUILayout.FloatField("Duration", att.impulses[i].duration);
                        EditorGUI.indentLevel--;
                    }

                    EditorGUI.indentLevel--;
                }
            }
            
            EditorGUILayout.Separator();
            DisplayAttackGroup(ref att.comboFold, ref att.ComboAttacks, ac, "Possible Combo Attacks");

            EditorGUI.indentLevel--;
        }
    }


}