using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum FormationShape{
    none,
    circle,
    square,
    randomSpread
}


/// <summary>
/// Gestiona los movimientos en formación
/// </summary>
public class Formation : MonoBehaviour
{

    public List<FormationMember> members = new List<FormationMember>();

    public NavMeshAgent agent;

    public FormationShape shape;

    public float spread = 2f;

    public float maxSpread = 8f;

    /// <summary>
    /// Ha habido cambios desde el último updates
    /// </summary>
    public bool isDirty;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDirty)
            applyChanges();
    }

    public void applyChanges(){
        var num = 0;
        foreach( var member in members){
            (member.formPosition, member.formRotation) = getPosition(num);
            num++;
        }
        isDirty = false;
    }

    public (Vector3?, Quaternion?) getPosition(int memberNum){

        if(shape == FormationShape.circle){
            int ringNum=1, levelPos =memberNum, ringPos ;

            //Encontrar en qué nivel del anillo le toca ponerse
            while(levelPos >= ( ringPos = ringPositions(ringNum))){
                levelPos -= ringPos;
                ringNum++;
            }
            var offset =  360 * levelPos / ringPos;
            var rotation = Quaternion.AngleAxis(offset, Vector3.up);
            var pos = (Vector3.forward )* ringRadius(ringNum);
            if (ringNum % 2 == 0)
                pos *= -1; //Alternar el lado en el que empiezan los círculos
            pos = rotation * pos;
            return (pos, rotation);
        
        }else if(shape == FormationShape.square){
            
            var offset = squarePosition(memberNum);

            var xPos = offset.x % 2 == 0 ? Vector3.right : Vector3.left;
            xPos *= (offset.x / 2 + 0.5f) * spread;
            var yPos = Vector3.forward * spread * offset.y;
            
            
            return (xPos + yPos, Quaternion.identity);
        }
        return (null, null);
    }

    /// <summary>
    /// Calcula cuantas posiciones posibles hay dentro de un nivel de la formación de anillo
    /// </summary>
    /// <param name="ringNum"></param>
    /// <returns></returns>
    public int ringPositions(int ringNum){
        
        return (int)(ringCircumference(ringNum) / spread);
    }

    /// <summary>
    /// Calcula la circumferencia de un nivel de los anillos
    /// </summary>
    /// <param name="ringNum"></param>
    /// <returns></returns>
    public float ringCircumference(int ringNum){
        return Mathf.PI * 2 * ringRadius(ringNum);
    }

    public float ringRadius(int ringNum){
        return ringNum  * spread * 0.8f - (ringNum-1)*0.5f ;
    }


    public Vector2Int squarePosition(int ringNum){
        var count = members.Count;
        var twice = count*2;
        Debug.Log(twice);
        int sizex = Mathf.CeilToInt(Mathf.Sqrt(twice))/2*2;
        int sizey =  sizex /2;
        Debug.Log($"{sizex} {sizey}");
        int x=0,y=1, n = 0;

        while (n < ringNum){
            n++;
            x++;
            if (x >= sizex) { x = 0; y++; }
        }
        
        return new Vector2Int(x,y);

    }


}
