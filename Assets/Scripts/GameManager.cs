using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int sceneToLoad; //Escena que toca cargar para pas�rsela a la escena de pantalla de carga
    public static int spaceshipDefaultHP = 250; //Vida est�ndar de la nave
    public static int spaceshipImprovedHP = 500; //Vida de la nave con la mejora adquirida
    public static int spaceshipDefaultLaunchTime = 10; //Tiempo de despegue est�ndar
    public static int spaceshipImprovedLaunchTime = 6; //Tiempo de despegue mejorado
    public static int spaceshipDefaultLootPenalty = 40; //Penalizaci�n est�ndar por despegue forzado
    public static int spaceshipImprovedLootPenalty = 10; //Penalizaci�n reducida por despegue forzado
    public static int astralitaTotal = 0; //Astralita total de la que dispone el jugador en la partida
    public static int numLevels = 2; //Niveles en el juego
    public static bool[] levelsState = new bool[numLevels]; //Estado de cada nivel (Bloqueado o Desbloqueado)
    public static int[] levelsMaxAstralita = new int[numLevels]; //Astralita m�xima que se ha conseguido en cada nivel
    public static string[] levelsLongestExpedition = new string[numLevels]; //Expedici�n m�s larga conseguida en cada nivel
    public static int[] astralitaUnlockCost = {0, 1000}; //Coste de desbloqueo de un nivel en Astralita
    public static int currentLevel = 0; //Nivel actual o �ltimo nivel que se ha jugado si no se est� jugando
    public static string[] levelsNames = { "Tar I", "Gaovin I" }; //Nombres de los planetas que representan los niveles
    public static bool[] shipUpgrades = new bool[3]; //Marca qu� habilidades de la rama de la nave est�n desbloqueadas
    public static string[] shipUpgradesTitle = { "Carcasa blindada", "Propulsores mejorados", "Bodega asegurada" }; //Titulo de las mejoras
    public static string[] shipUpgradesDescription = //Descripci�n de las mejoras
        { "Salud de la nave: " + spaceshipDefaultHP + " --> " + spaceshipImprovedHP,
          "Tiempo de despegue: " + spaceshipDefaultLaunchTime + "s --> " + spaceshipImprovedLaunchTime + "s",
          "Penalizaci�n por despegue forzado: " + spaceshipDefaultLootPenalty + "% --> " + spaceshipImprovedLootPenalty + "%"};
    public static int[] shipUpgradesCost = { 1, 3, 3 }; //Coste de las mejoras
}
