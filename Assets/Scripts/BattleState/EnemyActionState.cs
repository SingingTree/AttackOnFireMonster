using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BattleScenario
{
	class EnemyActionState : IBattleState
	{
		private float actionTime = 2.0f;
		private bool shotFired = false;
		private Image fireBall;
        public IBattleState UpdateState(BattleStateHandler battleStateHandler)
		{
			if (battleStateHandler.enemy.IsDead()) {
				return new EndGameState(true);
			}

			bool animationDone = shotFired && this.fireBall == null;

			if (animationDone || actionTime <= 0) {
				BattleAction enemyAction = new BattleAction(15);
				enemyAction.Apply(battleStateHandler.player);
				battleStateHandler.battleTextField.text = string.Format("JULIANA takes {0} damage", enemyAction.damage);
				return new CountdownState(battleStateHandler);
			}
			return this;
		}

		public void Update (BattleStateHandler battleStateHandler)
		{
			actionTime -= Time.deltaTime;

			if (battleStateHandler.enemy.IsDead()) {
				Color enemyColor = battleStateHandler.enemy.enemyImage.color;
				Color redEnemyColour = new Color (
					1.0f,
					0,
					0,
					enemyColor.a
					);

				battleStateHandler.enemy.enemyImage.color = redEnemyColour;
				return;
			}

			if (!shotFired) {
				this.fireBall = MonoBehaviour.Instantiate (battleStateHandler.enemyFireballPrefab) as Image;
				this.fireBall.transform.SetParent (battleStateHandler.canvas.transform, false);
				shotFired = true;
			}
			

		}
	}

}