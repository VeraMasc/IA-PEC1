# Runners

Implementados en [RunnerAi](/Assets/Scripts/AI/RunnerAi.cs). En el juego se ven como cápsulas de color rojo.

## Funcionamiento

Para los runners usamos una versión modificada del movimiento de los navmesh agents que unity usa por defecto para que implemente el suavizado por "ghost agent".

Concretamente, la modificación que hemos hecho altera como el objeto sigue el movimiento del agente simulado. En vez de hacer que la posición del Gameobject sea exactamente la misma que la del agente, hacemos que este siga al agente desde cierta distancia y siempre se aproxime a él a través del camino más corto. 

De esta forma logramos que los movimientos del agente, especialmente los giros, se traduzcan en movimientos más graduales por parte del Gameobject.

## Camino

Para describir el camino hemos usado una lista de puntos en el mapa y una clase ([Waypoints](/Assets/Scripts/AI/Waypoints.cs)) que permite a los Runners obtener la siguiente parada de su recorrido en función de su última parada visitada y el sentido en el que realizan el recorrido.
