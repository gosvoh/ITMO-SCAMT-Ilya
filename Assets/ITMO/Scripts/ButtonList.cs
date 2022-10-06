using UnityEngine;

namespace ITMO.Scripts
{
    public class ButtonList : MonoBehaviour
    {
        [SerializeField] private GameObject buttonTemplate;
        public void Start()
        {
            var node = Level.LevelNamesList.First;
            while (node != null)
            {
                var button = Instantiate(buttonTemplate, buttonTemplate.transform.parent, false);
                button.SetActive(true);
                button.GetComponent<Button>().SetText(node.Value);
                node = node.Next;
            }
            // for (int i = 0; i < Level.LevelList.Count; i++)
            // {
            //     GameObject button = Instantiate(buttonTemplate, buttonTemplate.transform.parent, false);
            //     button.SetActive(true);
            //     button.GetComponent<Button>().SetText(Level.LevelList.ToArray()[i]);
            // }
        }
    }
}