using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCObjectPool : MonoBehaviour
{
    List<GameObject> m_ObjectsPool;
    int m_CurrentElementId = 0;
    public TCObjectPool(int ElementsCount, GameObject Element)
    {
        m_ObjectsPool = new List<GameObject>();
        for (int i = 0; i < ElementsCount; i++)
        {
            GameObject l_Element = GameObject.Instantiate(Element);
            l_Element.SetActive(false);
            m_ObjectsPool.Add(l_Element);
        }

        m_CurrentElementId = 0;
    }

    public GameObject GetNextElement()
    {
        GameObject l_Element = m_ObjectsPool[m_CurrentElementId];
        ++m_CurrentElementId;
        if (m_CurrentElementId > m_ObjectsPool.Count)
        {
            m_CurrentElementId = 0;
        }
        return l_Element;
    }

}
