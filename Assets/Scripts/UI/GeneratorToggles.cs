using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Reflection;
using System.Text;

public class GenericBaseTest<T>
{

}

public class GenericDerived : GenericBaseTest<int>
{
}
public class GenericDerived2: GenericBaseTest<float>
{
}

public static class ExtensionHelpers
{
    public static bool IsGenericTypeWithArgs(this Type type, Type parentType, IEnumerable<Type> genericTypes)
    {
        if (type != null)
        {
            var baseType = type.BaseType;
            return (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition() == parentType &&
                baseType.GetGenericArguments().Any(t => genericTypes.Contains(t)));
        }
        return false;
    }
}

public class GeneratorToggles : MonoBehaviour
{
    [SerializeField]
    private Transform listRoot;

    [SerializeField]
    private GameObject togglePrefab;

    [SerializeField]
    private TextureGenerator generator;

    [SerializeField]
    private ToggleGroup group;

    private List<GameObject> buttons = new List<GameObject>();

    public void Generate(Type mazeType)
    {
        if (generator == null)
            Debug.LogError("GenerateToggle : TextureGenerator isn't set");

        if (listRoot == null)
            Debug.LogError("GenerateToggle : listRoot isn't set");

        if (togglePrefab == null)
            Debug.LogError("GenerateToggle : buttonPrefab isn't set");

        if (group == null)
            Debug.LogError("GenerateToggle : group isn't set");

        foreach (var item in buttons)
            DestroyImmediate(item);

        buttons.Clear();
        
        var mazeGeneratorTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsGenericTypeWithArgs(typeof(MazeGenerator<>), new[] { mazeType })).ToArray();
        foreach (var element in mazeGeneratorTypes)
        {
            var newButton = Instantiate(togglePrefab, listRoot);
            buttons.Add(newButton);
            newButton.GetComponentInChildren<Text>().text = element.ToString();
            newButton.GetComponent<Toggle>().onValueChanged.AddListener(c => { if (c) generator.SetGeneratorType(element); });
            newButton.GetComponent<Toggle>().group = group;
        }

        if (buttons.Count != 0)
            buttons.First().GetComponent<Toggle>().isOn = true;
    }
}
