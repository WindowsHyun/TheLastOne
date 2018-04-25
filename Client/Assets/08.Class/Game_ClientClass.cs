using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace TheLastOne.GameClass
{
    public class Game_ClientClass
    {
        private int id;             // 클라이언트 고유번호
        private int animator;    // 클라이언트 애니메이션
        private Vector3 position;  // 클라이언트 위치
        private Vector3 rotation;    // 클라이언트 보는 방향
        private Vector3 carrotation;    // 클라이언트 보는 방향
        private bool prefab;    // 클라이언트 프리팹이 만들어졌는지 확인
        private bool connect;    // 클라이언트 접속
        private string name;     // 클라이언트 닉네임
        private float horizontal;
        private float vertical;
        private int inCar;
        private bool removeClient;   // 클라이언트 지울경우 true
        private int nowWeaponState; // 클라이언트 무기 상태
        private bool activePlayer;
        public GameObject Player;   // 프리팹을 위한 게임 오브젝트
        public OtherPlayerCtrl script;  // 프리팹 오브젝트 안의 함수를 호출하기 위한 스크립트


        public int get_inCar() { return this.inCar; }
        public float get_vertical() { return this.vertical; }
        public float get_horizontal() { return this.horizontal; }
        public int get_id() { return this.id; }
        public int get_animator() { return this.animator; }
        public Vector3 get_pos() { return this.position; }
        public Vector3 get_rot() { return this.rotation; }
        public Vector3 get_car_rot() { return this.carrotation; }
        public bool get_prefab() { return this.prefab; }
        public bool get_activePlayer() { return this.activePlayer; }
        public bool get_connect() { return this.connect; }
        public string get_name() { return this.name; }
        public bool get_removeClient() { return this.removeClient; }
        public int get_weapon() { return this.nowWeaponState; }

        public void set_inCar(int value) { this.inCar = value; }
        public void set_vertical(float value) {  this.vertical = value; }
        public void set_horizontal(float value) { this.horizontal = value; }
        public void set_id(int value) { this.id = value; }
        public void set_animator(int value) { this.animator = value; }
        public void set_pos(Vector3 pos) { this.position = pos; }
        public void set_rot(Vector3 rot) { this.rotation = rot; }
        public void set_car_rot(Vector3 rot) { this.carrotation = rot; }
        public void set_prefab(bool value) { this.prefab = value; }
        public void set_activePlayer(bool value) { this.activePlayer = value; }
        public void set_connect(bool value) { this.connect = value; }
        public void set_name(string name) { this.name = name; }
        public void set_removeClient(bool value) { this.removeClient = value; }
        public void set_weapon(int value) { this.nowWeaponState = value; }

        public Game_ClientClass(int id, string name, Vector3 pos, Vector3 rot)
        {
            this.id = id;
            this.name = name;
            this.rotation = rot;
            this.animator = 0;
            this.connect = true;
            this.removeClient = false;
            this.Player = null;
            this.script = null;
            this.prefab = false;
            this.removeClient = false;
            this.activePlayer = true;
            this.nowWeaponState = 0;
            this.vertical = 0.0f;
            this.horizontal = 0.0f;
            this.inCar = -1;
        }

    }
}
