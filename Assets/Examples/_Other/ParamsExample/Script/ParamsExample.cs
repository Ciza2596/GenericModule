using Sirenix.OdinInspector;
using UnityEngine;

public class ParamsExample : MonoBehaviour
{
    [Button]
    private void Log()
    {
        Init(new[] { "hello1", "hello2" }, new[] { "Y1", "Y2" });
    }


    private void Init(params object[][] parametersList)
    {
        var parametersListContent = string.Empty;

        foreach (var parameters in parametersList)
        {
            foreach (var parameter in parameters)
            {
                parametersListContent += (string)parameter + ", ";
            }

            parametersListContent += "\n";
        }
        
        Debug.Log(parametersListContent);
    }
}