using IceCreamJam.Entities;
using Microsoft.Xna.Framework;
using Nez;
using System.Collections;
using System.Collections.Generic;

namespace IceCreamJam.RoadSystem {
	class CivilianCarSpawnSystem : SceneComponent {
		public int targetCarNumber = 10;
		public List<CivilianCar> cars;
		public List<Vector2> spawnPoints;
		private ICoroutine spawnRoutine;

		public CivilianCarSpawnSystem() {
			cars = new List<CivilianCar>();
		}

		public void Start(List<Vector2> spawnPoints) {
			this.spawnPoints = spawnPoints;
			spawnRoutine = Core.StartCoroutine(SpawnCar());
		}

		IEnumerator SpawnCar() {
			while(true) {
				yield return Coroutine.WaitForSeconds(1);
				TryToSpawnCar();
			}
		}

		public void TryToSpawnCar() {
			if(cars.Count >= targetCarNumber) {
				spawnRoutine.Stop();
				return;
			}
			
			foreach(Vector2 spawnPoint in spawnPoints.Shuffle()) {
				if(Physics.OverlapCircle(spawnPoint, 100) == null) {
					Scene.AddEntity(new CivilianCar(this) { Position = spawnPoint });
					return;
				}
			}
		}

		public void AddCar(CivilianCar vehicle) => cars.Add(vehicle);
		public void RemoveCar(CivilianCar vehicle) => cars.Remove(vehicle);
	}
}
