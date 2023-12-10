# Informe

1. [Controles](#controles)
2. [Apartados](#apartados)
    1. [Boids](#boids)
    2. [Behavior Trees](#behavior-trees)
        1. [Custom Bricks](#custom-bricks)
            1. [Actions](#actions)
            2. [Conditions](#conditions)
        2. [Extras](#extras)
    3. [Group Behavior](#group-behavior)


[Repositorio](https://github.com/VeraMasc/IA-PEC1)

[Video](https://youtu.be/MQNjHo94NJs)

## Controles

- Desplazar la cámara con WASD
- Orientar la cámara moviendo el ratón
- Zoom con la ruedecita del ratón

## Apartados

### Boids


### Behavior Trees

No voy a incluir el código en este apartado porque ocuparía una cantidad absurda de páginas. Esto se debe al funcionamiento del plugin que me ha obligado a crear multitud de scripts para realizar tareas básicas como asignar un valor o instanciar un prefab en una posición relativa al objeto actual ya que no venían de forma base en el plugin. Aviso también de que la documentación y comentarios de estos scripts no están muy elaborados, de nuevo, porque son demasiados y muchos de ellos hacen cosas básicas o han sido modificados una y otra vez por bugs con el plugin. Aún así, intentaré hacer un resumen breve de todos ellos.

#### Custom Bricks

Se encuentran todos en la carpeta Scripts/AI/Behaviors, en la subcarpeta correspondiente a su tipo

##### Actions

##### Conditions

#### Extras

Desde el principio tenía pensado hacer que el tema "poli vs ladron" fuera en realidad "perro vs limpiador" y que las cacas sirvieran como elemento de unión del primer y segundo apartado, pero quería una forma de poder influenciar el comportamiento de los otros agentes (que evitaran las cacas) que no requiriera ser hardcodeado en cada uno de los agentes (especialmente los de la práctica anterior), así que lo que he hecho es que las cacas modifiquen la navmesh.

Al principio intenté usar NavmeshObstacles pero eso generaba problemas con el que el limpiador limpiara las cacas, así que en vez de eso busqué una forma de modificar la navmesh en tiempo real y encontré una bastante simple y super eficiente, así que decidí copiarla y adaptarla un poco.

Al final las cacas han acabado siendo un Navmesh obstacle con un cilindro invisible debajo cuyo navmesh area es de tipo "Poop". Lo del cilindro es un apaño porque el navmeshModifierVolume solo permite rectángulos y no quería pelearme para ver cómo hacer literalmente la cuadratura del círculo con eso.


### Group Behavior

