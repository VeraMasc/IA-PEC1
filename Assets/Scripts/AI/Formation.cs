using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


/// <summary>
/// Formas posibles que puede tomar una formación
/// </summary>
public enum FormationShape{
    /// <summary>
    /// Sin forma
    /// </summary>
    none,
    /// <summary>
    /// Anillos concentricos alrededor del líder
    /// </summary>
    circle,

    /// <summary>
    /// Formación más o menos rectangular delante del líder (a lo pelotón)
    /// </summary>
    square
}


/// <summary>
/// Gestiona los movimientos en formación
/// </summary>
public class Formation : MonoBehaviour
{

    public List<FormationMember> members = new List<FormationMember>();

    public NavMeshAgent agent;

    /// <summary>
    /// Forma actual de la formación
    /// </summary>
    public FormationShape shape;

    /// <summary>
    /// Distancia de separación entre los miembros de la formación
    /// </summary>
    public float spread = 2f;

    /// <summary>
    /// Tolerancia en la comprobación de que cada miembro esté en su sitio, ver
    /// <see cref="FormationMember.isInPlace"/>
    /// </summary>
    public float tolerance = 1f;

    /// <summary>
    /// Mensaje que mostrar al cambiar a este modo de formación
    /// </summary>
    public Sprite[] shapeMessages;

    /// <summary>
    /// "Bocadillo" que usar para reproducir mensajes
    /// </summary>
    private SpeechBubble speechBubble;
    

    /// <summary>
    /// Ha habido cambios desde el último updates
    /// </summary>
    public bool isDirty;

    /// <summary>
    /// Indica que todos los miembros de la formación están en su lugar
    /// </summary>
    public bool allInPlace{
        get {
            return members.All(m => m.isInPlace);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        speechBubble = GetComponentInChildren<SpeechBubble>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDirty)
            applyChanges();
    }

    /// <summary>
    /// Aplica los cambios a los miembros de la formación
    /// </summary>
    public void applyChanges(){
        var num = 0;
        foreach( var member in members){
            (member.formPosition, member.formRotation) = getPosition(num);
            num++;
        }
        isDirty = false;
        speechBubble.say(shapeMessages[(int)shape]);
    }

    /// <summary>
    /// Obtiane la posición que le corresponde a un miembro de la formación
    /// </summary>
    /// <param name="memberNum">nº de miembro cuya posición queremos saber</param>
    /// <returns>Tupla con la posición y rotación correspondientes (null si no hay)</returns>
    public (Vector3?, Quaternion?) getPosition(int memberNum){

        if(shape == FormationShape.circle){ //Formación en círculo
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
        
        }else if(shape == FormationShape.square){ //Formación cuadrada
            
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
    /// <param name="ringNum">nº de anillo (desde dentro)</param>
    /// <returns></returns>
    public float ringCircumference(int ringNum){
        return Mathf.PI * 2 * ringRadius(ringNum);
    }

    /// <summary>
    /// Calcula el radio de un anilllo de la formación en círculo
    /// </summary>
    /// <param name="ringNum">nº de anillo (desde dentro)</param>
    /// <returns></returns>
    public float ringRadius(int ringNum){
        return ringNum  * spread * 0.8f - (ringNum-1)*0.5f ;
    }


    /// <summary>
    /// Calcula la forma de la formación cuadrada y obtiene las coordenadas del miembro indicado
    /// </summary>
    /// <param name="memberNum">número del miembro cuya posición queremos saber</param>
    /// <returns>"Coordenadas" del miembro (ALERTA! hay que convertirlas a distancias)</returns>
    public Vector2Int squarePosition(int memberNum){
        //Calcular medidas de la formación
        var count = members.Count;
        var twice = count*2;
        int sizex = Mathf.CeilToInt(Mathf.Sqrt(twice))/2*2;

        //Calcular coordenadas dentro de esas medidas
        int x=0,y=1, n = 0;
        while (n < memberNum){
            n++;
            x++;
            if (x >= sizex) { x = 0; y++; }
        }
        
        return new Vector2Int(x,y);

    }


}
