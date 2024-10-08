using Unity.Netcode;
using UnityEngine;

public class LinternaFunction : StageEnergyItemFunction
{
    [SerializeField] Light luz;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        luz.gameObject.SetActive(false);
    }

    protected override void switchState(int state)
    {
        base.switchState(state);
        luz.gameObject.SetActive(state == 0 ? false : true);
    }
}
