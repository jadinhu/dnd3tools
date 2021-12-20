/**
 * FacebookAndPlayFabFunctions.cs
 * Created by: Jadson Almeida [jadson.sistemas@gmail.com]
 * Created on: 31/08/17 (dd/mm/yy)
 * Revised on: 22/12/18 (dd/mm/yy)
 */
using UnityEngine;
using Facebook.Unity;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Facebook.MiniJSON;

public class FacebookAndPlayFabFunctions : MonoBehaviour
{
    //Title Id do seu jogo no site do PlayFab
    [SerializeField]
    string playfabTitleId;

    void Start()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        //Usado para inicializar o sdk do facebook.
        FB.Init(InitCallback, null, null);
        //Usado para indicar ao sdk do PlayFab o Id do seu jogo.
        PlayFabSettings.TitleId = playfabTitleId;
    }

    //Usado para realizar o login no facebook e no playfab ao mesmo tempo.
    public void LoginFacebook()
    {
        PlayfabLoginAndRegister.Instance.SetElementsInteractable(null, false);
        // Usado para verificar se o usu�rio j� est� logado, caso n�o esteja ele tenta logar.
            //Cria a lista de permiss�es que o aplicativo utilizar�, essas s�o as permiss�es padr�o.
            List<string> permissions = new List<string>() { "public_profile", "email", "user_friends" };

            //Utiliza o SDK do Facebook para realizar o login, utilizando as permiss�es e indicando a fun��o de callback.
            FB.LogInWithReadPermissions(permissions, LoginFacebookCallBack);
    }

    void LoginFacebookCallBack(ILoginResult loginResult)
    {
        //Caso o resultado seja nulo, deu algum erro no login.
        if (loginResult == null)
        {
            Debug.Log("N�o foi poss�vel logar no facebook.");
            PlayfabLoginAndRegister.Instance.SetElementsInteractable(null, false);
            return;
        }

        //Verifica se o retorno n�o foi um erro, ou algum tipo de cancelamento caso n�o seja nenhum desses tipos indica
        //que foi poss�vel realizar o login com o facebook com sucesso.
        if (string.IsNullOrEmpty(loginResult.Error) && !loginResult.Cancelled && !string.IsNullOrEmpty(loginResult.RawResult))
        {
            Debug.Log("Sucesso ao Logar no Facebook.");

            //Aqui verificamos se o usu�rio j� est� logado no PlayFab e caso n�o esteja tenta realizar o login.
            if (!FacebookAndPlayFabInfo.isLoggedOnPlayFab)
            {
                //O PlayFab possu� v�rios tipos de login, neste artigo vamos utilizar o Login com o facebook
                //ent�o nessa parte configuramos uma chamada para o PlayFab se Logar atrav�s dos dados do usu�rio
                //no facebook.
                LoginWithFacebookRequest loginFacebookRequest = new LoginWithFacebookRequest()
                {
                    //Indica o TitleId do seu jogo no PlayFab.
                    TitleId = PlayFabSettings.TitleId,
                    //Indica o token de acesso do usu�rio no Facebook, esse token s� � gerado se o usu�rio j� estiver
                    //logado no facebook
                    AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString,
                    //Indica para criar uma conta automaticamente dentro do seu jogo no PlayFab para este usu�rio, 
                    //caso ele j� n�o possua uma
                    CreateAccount = true
                };

                //Utiliza o SDK do PlayFab para realizar o login, utilizando a chamada e indicando as fun��es de callback
                //de sucesso e de error.
                PlayFabClientAPI.LoginWithFacebook(loginFacebookRequest, PlayFabLoginSucessCallBack, PlayFabErrorCallBack);
            }
        }
        else
        {
            PlayfabLoginAndRegister.Instance.SetElementsInteractable(null, true);
            Debug.Log("N�o foi poss�vel logar no facebook:\n" + loginResult.Error);
        }
    }

    void InitCallback()
    {
        //Verifica se foi poss�vel inicializar o sdk do facebook.
        if (FB.IsInitialized)
        {
            //Ativa o sdk do facebook no jogo.
            FB.ActivateApp();
        }
    }

    void PlayFabLoginSucessCallBack(PlayFab.ClientModels.LoginResult playfabLoginResult)
    {
        Debug.Log("Sucesso ao Logar no PlayFab.");

        //Armazena o Id do PlayFab na classe est�tica e com isso � poss�vel utilizar esses dados em outros scripts.
        FacebookAndPlayFabInfo.userPlayFabId = playfabLoginResult.PlayFabId;
        Debug.Log("PlayFabId: " + FacebookAndPlayFabInfo.userPlayFabId);

        //Utiliza o SDK do Facebook para consultar os dados, indicando a fun��o de callback. O valor "/me" indica
        //que estou buscando os dados do usu�rio que est� logado. O valor HttpMethod.GET indica que a nossa chamada ao
        //facebook tem a inten��o de somente coletar dados.
        FB.API("/me", HttpMethod.GET, CollectLoggedUserInfoCallback);
    }

    void CollectLoggedUserInfoCallback(IGraphResult result)
    {
        //Caso o resultado seja nulo, deu algum erro ao coletar os dados.
        if (result == null)
        {
            Debug.Log("N�o foi poss�vel coletar os dados do usu�rio no Facebook.");
            return;
        }

        //Verifica se o retorno n�o foi um erro, ou algum tipo de cancelamento caso n�o seja nenhum desses tipos indica
        //que foi poss�vel coletar os dados do facebook com sucesso.
        if (string.IsNullOrEmpty(result.Error) && !result.Cancelled && !string.IsNullOrEmpty(result.RawResult))
        {
            Debug.Log("Sucesso ao coletar os dados da conta do usu�rio no Facebook");

            try
            {
                //A resposta do Facebook vem em formato de Json e com isso n�s convertemos o Json para um Dictionary
                //para ser mais facil de coletar os dados
                Dictionary<string, object> dict = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
                string userFacebookName = dict["name"] as string;
                string userFacebookId = dict["id"] as string;

                //Exibe o Json de resposta no console da Unity
                Debug.Log(dict.ToJson());

                //Chamada usada para atualizar o nome do Usuario no playFab com o id do Facebook. Isto ir� permitir 
                //que futuramente seja poss�vel coletar as informa��es desse usu�rio ao exibir o resultado do Leaderboard.
                //Neste ponto foi utilizado o Id do Facebook ao inv�s do Nome do usu�rio no Facebook porque o campo
                //Display Name no PlayFab deve ser �nico e n�o passar de 25 caracters. Sendo assim o Id do facebook
                //nos atende muito bem pois ele � um id �nico por usu�rio e n�o ultrapassa esse limite.
                UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest()
                {
                    DisplayName = userFacebookId
                };
                PlayFabClientAPI.UpdateUserTitleDisplayName(request, UpdateUserTitleDisplayNameSucessCallback, PlayFabErrorCallBack);

                //Atualiza as informa��es da classe est�tica indicando que o usu�rio est� logado no PlayFab
                //e informando o Id do facebook e o nome do usu�rio. Com isso � poss�vel utilizar esses dados em outros scripts.
                FacebookAndPlayFabInfo.isLoggedOnPlayFab = true;
                FacebookAndPlayFabInfo.userFacebookId = userFacebookId;
                FacebookAndPlayFabInfo.userName = userFacebookName;

                PlayfabLoginAndRegister.Instance.LoadSpellbook();
            }
            //Usado caso o Facebook n�o tenha retornado o id ou o nome do usu�rio.
            catch (KeyNotFoundException e)
            {
                Debug.Log("N�o foi poss�vel coletar os dados do usu�rio no Facebook. Erro: " + e.Message);
            }
        }
        else
            Debug.Log("N�o foi poss�vel coletar os dados do usu�rio no Facebook.");
    }

    void UpdateUserTitleDisplayNameSucessCallback(UpdateUserTitleDisplayNameResult result)
    {
        //Exibe no console da Unity que atualizou o campo com sucesso.
        Debug.Log("O campo Display Name do usu�rio no PlayFab foi atualizado com sucesso.");
    }

    void PlayFabErrorCallBack(PlayFabError playfabError)
    {
        //Exibe no console da Unity as informa��es do erro.
        Debug.Log(playfabError.ErrorMessage);
        Debug.Log(playfabError.ErrorDetails);
    }
}