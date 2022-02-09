using IceCreamJam.RoadSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCreamJam.Source.RoadSystem {
	interface IVehicle {
		Node PreviousNode { get; set; }
		Node CurrentNode { get; set; }
		Node NextNode { get; set; }
		Direction8 currentDirection { get; set; }

		event Action OnTurnFinished;

		void StartTurn();
	}
}
