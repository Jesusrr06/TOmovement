/// <summary>
/// Simple static container for selected character indices. Persisted per run.
/// </summary>
public static class GameData
{
    // Inicializar a -1 para detectar no-selección explícita en la pantalla de selección
    public static int Player1Character = -1;
    public static int Player2Character = -1;
}