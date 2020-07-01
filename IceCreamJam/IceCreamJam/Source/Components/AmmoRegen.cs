using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCreamJam.Source.Components {
    class AmmoRegen : Component, IUpdatable {
        AmmoComponent ammo;
        public override void OnAddedToEntity() {
            ammo = Entity.GetComponent<AmmoComponent>();
        }

        public void Update() {
            ammo.Reload(1);
        }
    }
}
