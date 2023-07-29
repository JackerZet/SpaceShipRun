using Characters;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Main
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        public class MessageName : MessageBase
        {
            public string name;
            public override void Serialize(NetworkWriter writer) => writer.Write(name);
            public override void Deserialize(NetworkReader reader) => name = reader.ReadString();
        }

        [SerializeField] private TMP_InputField _inputField;

        private Dictionary<int, ShipController> _players = new Dictionary<int, ShipController>();

        private const int MESSAGE_TYPE = 100;
        public override void OnServerAddPlayer(NetworkConnection connect, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();

            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);

            _players.Add(connect.connectionId, player.GetComponent<ShipController>());
            NetworkServer.AddPlayerForConnection(connect, player, playerControllerId);
        }

        public override void OnClientConnect(NetworkConnection connect)
        {
            base.OnClientConnect(connect);
            MessageName login = new MessageName { name = _inputField.text};         
            connect.Send(MESSAGE_TYPE, login);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler(MESSAGE_TYPE, ReceiveLogin);
        }

        public void ReceiveLogin(NetworkMessage message)
        {
            _players[message.conn.connectionId].PlayerName = message.reader.ReadString();
            _players[message.conn.connectionId].gameObject.name = _players[message.conn.connectionId].PlayerName;
        }

    }
}
