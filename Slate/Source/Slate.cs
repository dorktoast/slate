using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using UnityEngine.UI;

namespace GIB
{
    public class Slate : UdonSharpBehaviour
    {
        [SerializeField] private Animator animator;

        [SerializeField] private TextMeshProUGUI timeValue;
        [SerializeField] private TextMeshProUGUI dateValue;
        [SerializeField] private TextMeshProUGUI dateValueBack;
        [SerializeField] private TextMeshProUGUI fpsValue;
        [SerializeField] private TextMeshProUGUI takeValue;

        [SerializeField] private TextMeshProUGUI takeLog;
        [SerializeField] private InputField takeLogCopy;
        [SerializeField] private InputField takeNote;

        [SerializeField] private InputField rollInput;
        [SerializeField] private InputField sceneInput;
        [SerializeField] private InputField camInput;
        [SerializeField] private Toggle mosToggle;
        [SerializeField] private Toggle syncToggle;

        private int take = 1;

        private void Start()
        {
            DateTime myDate = DateTime.Now;
            string formattedDate = myDate.ToString("MMM-dd-yyyy").ToUpper();
            dateValue.text = formattedDate;
            dateValueBack.text = $"CAM {camInput.text}: {formattedDate}";
        }

        private void FixedUpdate()
        {
            DateTime myTime = DateTime.Now;
            string formattedTime = myTime.ToString("HH:mm:ss.ff");
            timeValue.text = formattedTime;
            fpsValue.text = (1 / Time.fixedDeltaTime).ToString("F1");
        }

        public override void OnPickupUseDown()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DoClap");
        }

        public void DoClap()
        {
            animator.SetTrigger("doClap");
            SendCustomEventDelayedSeconds("DoMarkLine", 1f);
            SendCustomEventDelayedSeconds("ResetSlate", 6f);
        }

        public void ResetSlate()
        {
            animator.ResetTrigger("doClap");
            AddTake();
        }

        public void DoMarkLine()
        {
            string toLog = takeLog.text + "\n"+ GetMarkLine();
            takeLog.text = toLog;
            takeLogCopy.text = toLog;

            DateTime myDate = DateTime.Now;
            string formattedDate = myDate.ToString("MMM-dd-yyyy").ToUpper();
            dateValue.text = formattedDate;
            dateValueBack.text = $"CAM {camInput.text}: {formattedDate}";
        }

        public void ResetTakes()
        {
            take = 0;
            takeValue.text = take.ToString();
        }

        public void AddTake()
        {
            take++;
            takeValue.text = take.ToString();
        }

        public void RemoveTake()
        {
            take--;
            takeValue.text = take.ToString();
        }

        public void MarkGood()
        {
            takeLog.text += "GOOD";
            takeLogCopy.text = takeLog.text;
        }

        public void MarkBad()
        {
            takeLog.text += "BAD";
            takeLogCopy.text = takeLog.text;
        }

        public void MarkOk()
        {
            takeLog.text += "OK";
            takeLogCopy.text = takeLog.text;
        }

        public void AddTakeNote()
        {
            string toLog = $"{takeLog.text} ({takeNote.text})";
            takeLog.text = toLog;
            takeLogCopy.text = toLog;
            takeNote.text = "";
        }

        private string GetMarkLine()
        {
            string mosString = mosToggle.isOn ? " MOS" : "";
            string syncString = syncToggle.isOn ? " SYNC" : "";

            return $" C {camInput.text}/R {rollInput.text}/S {sceneInput.text}/T {take} /{mosString}{syncString} ";
        }
    }
}
