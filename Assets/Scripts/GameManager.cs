using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int astralitaTotal = 150; //Astralita total de la que dispone el jugador en la partida
    public static int numLevels = 2; //Niveles en el juego
    public static bool[] levelsState = new bool[numLevels]; //Estado de cada nivel (Bloqueado o Desbloqueado)
    public static int[] levelsMaxAstralita = new int[numLevels]; //Astralita máxima que se ha conseguido en cada nivel
    public static string[] levelsLongestExpedition = new string[numLevels]; //Expedición más larga conseguida en cada nivel
    public static int[] astralitaUnlockCost = {0, 100}; //Coste de desbloqueo de un nivel en Astralita
    public static int currentLevel = 0; //Nivel actual o último nivel que se ha jugado si no se está jugando
    public static string[] levelsNames = { "Kepler-11b", "Kepler-11c" }; //Nombres de los planetas que representan los niveles
}
