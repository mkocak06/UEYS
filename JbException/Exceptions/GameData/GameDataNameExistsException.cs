using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.GameData
{
    public class GameDataNameExistsException : JbBaseException
    {
        public GameDataNameExistsException(string gameDataName) : base($"The game data data named {gameDataName} already exists.")
        {
        }

        public GameDataNameExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "GAME_DATA-NAME-EXISTS";
    }
}
