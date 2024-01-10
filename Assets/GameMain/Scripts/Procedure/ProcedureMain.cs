using System.Collections;
using System.Collections.Generic;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;

public class ProcedureMain : ProcedureBase {
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner) {
        base.OnEnter(procedureOwner);

        GameEntry.UI.OpenUIForm(UIFormId.PuzzleForgeForm, this);
    }
}
