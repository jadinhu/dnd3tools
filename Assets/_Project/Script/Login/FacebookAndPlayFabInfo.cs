/**
 * FacebookAndPlayFabInfo.cs
 * Created by: Jadson Almeida [jadson.sistemas@gmail.com]
 * Created on: 31/08/17 (dd/mm/yy)
 * Revised on: 31/08/17 (dd/mm/yy)
 */
public static class FacebookAndPlayFabInfo
{
    //Id do usu�rio no PlayFab
    public static string userPlayFabId = "";
    //Id do usu�rio no Facebook
    public static string userFacebookId = "";
    //Nome do usu�rio no Facebook
    public static string userName = "";
    //Indica se o usu�rio j� est� logado no PlayFab
    public static bool isLoggedOnPlayFab = false;
    //Indica se os dados do leaderboard j� foram carregados.
    public static bool leaderboardLoaded = false;
    //Indica se os dados do leaderboard est�o sendo carregados.
    public static bool leaderboardIsLoading = false;
}