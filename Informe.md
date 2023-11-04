# README
1. [Controles](#controles)
2. [Apartados](#apartados)
   1. [Runners](#runners)
   2. [Abuelos](#abuelos)
      1. [Estados](#estados)
         1. [Cambios de estado](#cambios-de-estado)
         2. [Wander](#wander)
         3. [GoToBench](#gotobench)
         4. [Rest](#rest)
         5. [LeaveBench](#leavebench)
            1. [Código Abuelos](#código-abuelos)
         6. [GrandpaState](#grandpastate)
         7. [Código Ejecución Estados](#código-ejecución-estados)

## Controles

- Desplazar la cámara con WASD
- Orientar la cámara moviendo el ratón
- Zoom con la ruedecita del ratón

## Apartados

### Runners

[Runners](/Documentation/Runners.md)

### Abuelos
Implementados en [GrandpaAi](/Assets/Scripts/AI/GrandpaAI.cs). En el juego se ven como cápsulas de color morado.

#### Estados

Los abuelos tienen 4 estados definidos en el enum [GrandpaState](#GrandpaState). Estos son:

0. [Wander](#Wander)
1. [GoToBench](#GoToBench)
2. [Rest](#Rest)
3. [LeaveBench](#LeaveBench)

**DIAGRAMA**

##### Cambios de estado

Por cuestiones de simplicidad y eficiencia, los cambios de estado no se calculan en todo momento, sino solo cuando el estado actual ha finalizado su ejecución. Es decir, que o bien el agente ha llegado a su destino, o el tiempo de descanso en el banco se ha finalizado.

##### Wander

Es el estado base de los abuelos en el cual lo que hacen es simplemente deambular de forma aleatoria por el escenario. Para lograrlo, le damos al agente una posición de destino aleatoria pero relativamente cercana.

Una vez llegue a la posición de destino, ejecutará el cálculo del siguiente estado. Si encuentra un banco cerca, cambiará a [GoToBench](#GoToBench), si no, reiniciará el estado [Wander](#Wander).



##### GoToBench

En este estado lo que hacen es ir al banco más cercano e intentar "sentarse" en él. Hasta que detectan que están lo suficientemente cerca de la posición objetivo y que se encuentan encima de un banco (una navmesh area de tipo Bench), entonces pasan al estado [Rest](#Rest).

##### Rest

Este es el estado en el que el abuelo se queda "descansando" en un banco. Para lograr este efecto sin que los abuelos se salgan del banco lo que hacemos es resetear el path del agent para que deje de moverse y le limitamos las navmesh areas por las que se puede mover a únicamente el área de los bancos.

De esta forma, aunque otro agente intente sentarse en el mismo banco, no se verá empujado fuera de este a la vez que tampoco se quedará "bloqueado" en el sitio sin dejar que otros agentes se sienten. Simplemente se dejará empujar a un lado del banco, dando la sensación de que le está "haciendo hueco" al otro agente para que se siente.

Una vez pasado el tiempo indicado de descanso, el estado cambia automáticamente a [LeaveBench](#LeaveBench)

##### LeaveBench

Este estado es casi indistinguible de Wander salvo por un leve matiz: cuando detecta un banco cerca, no se sienta, ya que el objetivo del estado es precisamente evitar posibles bucles en los que un abuelo no hace nada más que levantarse y volverse a sentar en el mismo banco o en otro cercano.

De no encontrar ningún banco cerca, el estado cambia a [Wander](#Wander), de lo contrario, se mantiene en [LeaveBench](#LeaveBench) hasta "dejar atrás" los bancos y poder deambular un poco antes de volver a sentarse.

Podría haber implementado esta solución al problema de los abuelos que entran en bucle de muchas otras formas, desde probabilidades en el cambio de estado, hasta un "cooldown", una probablilidad que aumentara con el tiempo, etc. Pero ya que se hacía mucho hincapié en las máquinas de estados he pensado que añadir este estado era la más adecuada para este ejercicio.

###### Código Abuelos

> Por motivos de simplicidad y legiblidad, recomendaría mirar directamente el código en [GrandpaAi](/Assets/Scripts/AI/GrandpaAI.cs). Está todo comentado y resulta mucho más legible

La ejecución de los estados frame por frame es realizada por **executeState**, pero en general estos no hacen gran cosa a parte de comprovar si se puede dar el estado actual por finalizado.

Lo verdaderamente importante son la funcione que decide el próximo estado (**getNextState**) y la que lo inicializa (**initializeState**), ya que la mayor parte del movimiento en sí está gestionada por el propio sistema de navmesh agents de unity.

##### GrandpaState

```cs
    public enum GrandpaState{
    /// <summary>
    /// Deambulando
    /// </summary>
    wander,
    /// <summary>
    /// Yendo a descansar en un banco
    /// </summary>
    goToBench,
    /// <summary>
    /// Descansando en un banco
    /// </summary>
    rest,
    /// <summary>
    /// Yendo a descansar en un banco
    /// </summary>
    leaveBench,
}
```

##### Código Ejecución Estados

```cs
    /// <summary>
    /// Ejecuta el estado actual de la FSM
    /// </summary>
    private void executeState()
    {
        //Cambiar de estado automáticamente al terminar el path actual
        if(stateNeedsUpdate())
            state = getNextState();

        switch(state){
            case GrandpaState.leaveBench:
            case GrandpaState.wander:
                wander(); break;

            case GrandpaState.rest:
                rest(); break;
        }

        
    }

    /// <summary>
    /// Ejecuta el estado de deambular
    /// </summary>
    public void wander(){
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
            return;
        
        var randomVector = Quaternion.FromToRotation(Vector3.forward,Vector3.up) * (Vector3)(Random.insideUnitCircle * wanderRange) + agent.velocity * wanderInertia;
        var minVector = randomVector.normalized * minWanderRange;
        if(minVector.magnitude > randomVector.magnitude) //Minimum movement
            randomVector = minVector; 
        agent.destination =  randomVector + transform.position;		
        
        
            
    }

    /// <summary>
    /// Ejecuta el estado de descanso
    /// </summary>
    public void rest(){
        restRemaining -= Time.deltaTime;
    }
```