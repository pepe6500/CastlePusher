using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BallGame
{
    public class Castle : NetworkBehaviour
    {
        int hp = 3;
        public ParticleSystem deadParticle;
        public UnityEngine.UI.Image hpBar;
        public BallGameManager ballGameManager;
        [SyncVar] public int index;

        public void SetHP(int num)
        {
            Debug.Log(index + "  : HP = " + hp);
            if (hp > 0 && num <= 0)
            {
                Dead();
                hp = 0;
                hpBar.fillAmount = 0;
                return;
            }
            else if(hp <= 0 && num > 0)
            {
                deadParticle.Stop();
            }
            hp = num;
            hpBar.fillAmount = hp / 3f;
        }
        
        public void RpcOnAttack(int pow)
        {
            hp -= pow;
            if (hp <= 0)
            {
                hp = 0;
                Dead();
            }
            hpBar.fillAmount = hp / 3f;
        }
        
        public int GetHP()
        {
            return hp;
        }

        void Dead()
        {
            deadParticle.Play();
        }
    }
}
