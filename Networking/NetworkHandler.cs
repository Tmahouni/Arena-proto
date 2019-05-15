using UnityEngine;

public class NetworkHandler : MonoBehaviour {
	
        public Player PlayerPrefab;
        public GameObject Ninja;
        public GameObject Angel;
        
        void Update()
        {
		
        }
	
        //Call this when creating server
        /* public static void StartServer(string Servername)
        {
            Network.InitializeServer(32,25000,!Network.HavePublicAddress() );
            MasterServer.RegisterHost("Arena_battle",Servername,"ZB network testi");
        }*/
	
	
        //Old  
        /*public void SpawnPlayer()
        {
            Destroy(Camera.main); //Remove old Main Camera

            Player player = Network.Instantiate(PlayerPrefab, new Vector3(100,100,100), Quaternion.identity, 0) as Player;

            if(player.networkView.isMine)
            { //check to make sure it’s our player being spawned
                Transform playerCam = player.transform.Find("Camera"); //find child named “Camera”
                playerCam.tag = "MainCamera"; //set it’s tag so it becomes the active one
            }
		
            //Set up player attributes here
            player.ID = Random.Range(0,100);
            GameLogic.lastPlayerId++;
		
            Debug.Log("Spawning player");
        }*/

        public void PlayerFactory(string @class, string team,string name)
        {
            Destroy(Camera.main); //Remove old Main Camera
            GameObject player_GO = null;

            if(@class == "Ninja")
                 player_GO = Network.Instantiate(Ninja, GameLogic.SpawnPoints[0], Quaternion.identity, 0) as GameObject;
            else player_GO = Network.Instantiate(Angel, GameLogic.SpawnPoints[0], Quaternion.identity, 0) as GameObject;
            //Player player = Network.Instantiate(class_proto, GameLogic.SpawnPoints[0], Quaternion.identity, 0) as Player;

            var player = player_GO.GetComponent<Player>();

            if (player.networkView.isMine)
            { //check to make sure it’s our player being spawned
                Transform playerCam = player.transform.Find("Camera"); //find child named “Camera”
                playerCam.tag = "MainCamera"; //set it’s tag so it becomes the active one
            }

            

            //Set up player attributes here
            player.ID = GameLogic.lastPlayerId;
            player.team = team;
            player.@class = @class;
            player.p_Name = name;
			player.ReSpawn();
            GameLogic.lastPlayerId++;
            Debug.Log("Spawning player with " + @class);
        }
	
	
	
        //Monobehavior function, don't call
        void OnServerInitialized()
        {
            Debug.Log("Server initialized!");
            //SpawnPlayer();	
        }
	
        //same
        void OnConnectedToServer()
        {
            Debug.Log("OnConnected");
            //SpawnPlayer();
        }
	
        //same
        void OnMasterServerEvent(MasterServerEvent mse)
        {
            if(mse == MasterServerEvent.RegistrationSucceeded)
            {
                Debug.Log("Server registered");
            }
            else 
			Debug.Log("Couldn't register server..");
        }

        void OnPlayerDisconnected(NetworkPlayer player)
        {
            Network.RemoveRPCs(player);
            Network.DestroyPlayerObjects(player);
        }
    }


