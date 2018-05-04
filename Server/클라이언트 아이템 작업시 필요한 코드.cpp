//-----------------------------------------------------------------------------------------------------------------
            // 화면 체크를 위한 키 등록
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (sensorCheck == false)
                {
                    FollowCam followCam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
                    followCam.change = true;
                    Debug.Log("체크 인");
                    followCam.height = 2.5f;
                    followCam.dist = 7.0f;
                    sensorCheck = true;
                }
                else
                {
                    FollowCam followCam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
                    followCam.change = false;
                    Debug.Log("체크 아웃");
                    followCam.height = 35.0f;
                    followCam.dist = 25.0f;
                    sensorCheck = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                string filePath = "./item_collection.txt";
                string coordinates = "";
                coordinates = "name:M16|x:" + tr.position.x + "|y:" + tr.position.y + "|z:" + tr.position.z + "|" + System.Environment.NewLine;
                System.IO.File.AppendAllText(filePath, coordinates);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                string filePath = "./item_collection.txt";
                string coordinates = "";
                coordinates = "name:556|x:" + tr.position.x + "|y:" + tr.position.y + "|z:" + tr.position.z + "|" + System.Environment.NewLine;
                System.IO.File.AppendAllText(filePath, coordinates);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                string filePath = "./item_collection.txt";
                string coordinates = "";
                coordinates = "name:AK47|x:" + tr.position.x + "|y:" + tr.position.y + "|z:" + tr.position.z + "|" + System.Environment.NewLine;
                System.IO.File.AppendAllText(filePath, coordinates);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                string filePath = "./item_collection.txt";
                string coordinates = "";
                coordinates = "name:762|x:" + tr.position.x + "|y:" + tr.position.y + "|z:" + tr.position.z + "|" + System.Environment.NewLine;
                System.IO.File.AppendAllText(filePath, coordinates);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                string filePath = "./item_collection.txt";
                string coordinates = "";
                coordinates = "name:M4A1|x:" + tr.position.x + "|y:" + tr.position.y + "|z:" + tr.position.z + "|" + System.Environment.NewLine;
                System.IO.File.AppendAllText(filePath, coordinates);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                string filePath = "./item_collection.txt";
                string coordinates = "";
                coordinates = "name:UMP|x:" + tr.position.x + "|y:" + tr.position.y + "|z:" + tr.position.z + "|" + System.Environment.NewLine;
                System.IO.File.AppendAllText(filePath, coordinates);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                string filePath = "./item_collection.txt";
                string coordinates = "";
                coordinates = "name:9mm|x:" + tr.position.x + "|y:" + tr.position.y + "|z:" + tr.position.z + "|" + System.Environment.NewLine;
                System.IO.File.AppendAllText(filePath, coordinates);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                string filePath = "./item_collection.txt";
                string coordinates = "";
                coordinates = "name:AidKit|x:" + tr.position.x + "|y:" + tr.position.y + "|z:" + tr.position.z + "|" + System.Environment.NewLine;
                System.IO.File.AppendAllText(filePath, coordinates);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                string filePath = "./item_collection.txt";
                string coordinates = "";
                coordinates = "name:Armor|x:" + tr.position.x + "|y:" + tr.position.y + "|z:" + tr.position.z + "|" + System.Environment.NewLine;
                System.IO.File.AppendAllText(filePath, coordinates);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                string filePath = "./item_collection.txt";
                string coordinates = "";
                coordinates = "name:UAZ|x:" + tr.position.x + "|y:" + tr.position.y + "|z:" + tr.position.z + "|" + System.Environment.NewLine;
                System.IO.File.AppendAllText(filePath, coordinates);
            }
            //-----------------------------------------------------------------------------------------------------------------