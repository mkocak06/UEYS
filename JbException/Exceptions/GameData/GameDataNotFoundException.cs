using JbException.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JbException.Exceptions.GameData
{
    public class GameDataNotFoundException : JbBaseException
    {
        public GameDataNotFoundException(string gameDataId) : base($"The game data Id {gameDataId} not found.")
        {
        }

        public GameDataNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override string ErrorCode => "GAME-DATA-NOT-FOUND";
    }
}
