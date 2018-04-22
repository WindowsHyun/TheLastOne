using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace TheLastOne.GameClass
{
    public class Game_ZombieClass
    {
        private int id;             // 클라이언트 고유번호
        private int animator;    // 클라이언트 애니메이션
        private Vector3 position;  // 클라이언트 위치
        private Vector3 rotation;    // 클라이언트 보는 방향
        private bool prefab;    // 클라이언트 프리팹이 만들어졌는지 확인
        public GameObject Zombie;   // 프리팹을 위한 게임 오브젝트
        public ZombieCtrl script;  // 프리팹 오브젝트 안의 함수를 호출하기 위한 스크립트

        public int get_id() { return this.id; }
        public int get_animator() { return this.animator; }
        public Vector3 get_pos() { return this.position; }
        public Vector3 get_rot() { return this.rotation; }
        public bool get_prefab() { return this.prefab; }

        public void set_id(int value) { this.id = value; }
        public void set_animator(int value) { this.animator = value; }
        public void set_pos(Vector3 pos) { this.position = pos; }
        public void set_rot(Vector3 rot) { this.rotation = rot; }
        public void set_prefab(bool value) { this.prefab = value; }

        public Game_ZombieClass(int id, Vector3 pos, Vector3 rot)
        {
            this.id = id;
            this.rotation = rot;
            this.animator = 0;
            this.Zombie = null;
            this.script = null;
            this.prefab = false;
        }
    }
}
