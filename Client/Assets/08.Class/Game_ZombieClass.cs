using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace TheLastOne.GameClass
{
    public class Game_ZombieClass
    {
        private int id;             // 좀비 고유번호
        private int hp;             // 좀비 HP
        private int animator;    // 좀비 애니메이션
        private int target_player;  // 좀비 타겟 플레이어
        private Vector3 position;  // 좀비 위치
        private Vector3 rotation;    // 좀비 보는 방향
        private bool prefab;    // 좀비 프리팹이 만들어졌는지 확인
        private bool removeZombie;   // 좀비를 지울경우 true
        private bool activeZombie;  // 좀비 활성화
        public GameObject Zombie;   // 프리팹을 위한 게임 오브젝트
        public ZombieCtrl script;  // 프리팹 오브젝트 안의 함수를 호출하기 위한 스크립트

        public int get_hp() { return this.hp; }
        public int get_id() { return this.id; }
        public int get_animator() { return this.animator; }
        public int get_target() { return this.target_player; }
        public Vector3 get_pos() { return this.position; }
        public Vector3 get_rot() { return this.rotation; }
        public bool get_prefab() { return this.prefab; }
        public bool get_removeZombie() { return this.removeZombie; }
        public bool get_activeZombie() { return this.activeZombie; }

        public void set_id(int value) { this.id = value; }
        public void set_hp(int value) { this.hp = value; }
        public void set_animator(int value) { this.animator = value; }
        public void set_target(int value) { this.target_player = value; }
        public void set_pos(Vector3 pos) { this.position = pos; }
        public void set_rot(Vector3 rot) { this.rotation = rot; }
        public void set_prefab(bool value) { this.prefab = value; }
        public void set_removeZombie(bool value) { this.removeZombie = value; }
        public void set_activeZombie(bool value) { this.activeZombie = value; }

        public Game_ZombieClass(int id, int target, Vector3 pos, Vector3 rot)
        {
            this.id = id;
            this.rotation = rot;
            this.animator = 0;
            this.target_player = target;
            this.Zombie = null;
            this.script = null;
            this.prefab = false;
            this.removeZombie = false;
            this.activeZombie = true;
        }
    }
}
