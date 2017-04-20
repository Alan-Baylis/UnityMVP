using UnityEditor;
using Becerra.MVP.Views;
using Becerra.MVP.Model;

namespace Becerra.MVP.Editor
{
    [CustomEditor(typeof(View<IModel>), true)]
    public class ViewInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Hello world");
        }
    }
}
