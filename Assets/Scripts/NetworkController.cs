﻿using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.SmokeMonkey.PowerPong
{
    public class NetworkController : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields
		[Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
		[SerializeField]
		private byte maxPlayersPerRoom = 2;
		[Tooltip("The Ui Panel to let the user enter name, connect and play")]
		[SerializeField]
		private GameObject controlPanel;
		[Tooltip("The UI Label to inform the user that the connection is in progress")]
		[SerializeField]
		private GameObject progressLabel;
		bool isConnecting;

        #endregion

        #region Private Fields

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "0.0.1";

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
			GameObject.DontDestroyOnLoad(this);
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
			progressLabel.SetActive(false);
			controlPanel.SetActive(true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
			// keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
			isConnecting = true;
			progressLabel.SetActive(true);
			controlPanel.SetActive(false);
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
				Debug.Log("Connected");
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
				Debug.Log("Not Connected");
            }
        }
		#endregion
		
		#region MonoBehaviourPunCallbacks Callbacks

		public override void OnConnectedToMaster()
		{
			Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
			if (isConnecting)
			{
				PhotonNetwork.JoinRandomRoom();
			}
		}

		public override void OnDisconnected(DisconnectCause cause)
		{
			progressLabel.SetActive(false);
			controlPanel.SetActive(true);
			Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
		}
		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

			// #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
			PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
		}

		public override void OnJoinedRoom()
		{
			Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
			if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
			{
				Debug.Log("First Player Joined. Waiting for another player.");
				PhotonNetwork.LoadLevel("Room for 2");
								Debug.Log("aaa");


			}
			else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
			{
				Debug.Log("Second Player Joined. Loading scene.");
				PhotonNetwork.LoadLevel("Room for 2");
								Debug.Log("bbb");

			}
			else
			{
				//!!!!change this to find another room!!!!
				Debug.Log("Too many players in this room. Disconnecting");
				PhotonNetwork.LeaveRoom();
				
			}
		}
		#endregion
	
    }
}