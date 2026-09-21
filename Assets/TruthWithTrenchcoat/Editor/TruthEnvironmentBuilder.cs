using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public static class TruthEnvironmentBuilder
{
    static Transform root;
    static Material wall, floor, darkMetal, glass, blueGlow, white, wood, black, accent, blood;

    [MenuItem("Truth With Trenchcoat/Build Detailed Environment")]
    public static void Build()
    {
        if (GameObject.Find("TRUTH_ENVIRONMENT"))
            Object.DestroyImmediate(GameObject.Find("TRUTH_ENVIRONMENT"));
        var go = new GameObject("TRUTH_ENVIRONMENT"); root = go.transform;
        MakeMaterials();
        BuildArchitecture();
        BuildRooms();
        BuildLighting();
        BuildPreviewCamera();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("Truth With Trenchcoat environment built. This is a detailed procedural art/blockout pass using Unity primitives; replace selected props with final meshes later.");
        Selection.activeGameObject = go;
    }

    static void MakeMaterials()
    {
        wall=Mat("Wall",new Color(.16f,.18f,.20f)); floor=Mat("Floor",new Color(.08f,.09f,.10f)); darkMetal=Mat("Metal",new Color(.10f,.12f,.14f)); glass=Mat("Glass",new Color(.08f,.16f,.19f)); blueGlow=Mat("BlueGlow",new Color(.03f,.25f,.55f),true); white=Mat("White",new Color(.65f,.68f,.70f)); wood=Mat("Wood",new Color(.20f,.12f,.07f)); black=Mat("Black",new Color(.015f,.018f,.02f)); accent=Mat("Accent",new Color(.28f,.32f,.35f)); blood=Mat("ClueDecal",new Color(.22f,.015f,.01f));
    }
    static Material Mat(string n, Color c, bool emission=false){var m=new Material(Shader.Find("Standard"));m.name=n;m.color=c;if(emission){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",c*2.5f);}return m;}
    static GameObject Cube(string n, Vector3 p, Vector3 s, Material m, Transform par=null){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(par??root);g.transform.localPosition=p;g.transform.localScale=s;g.GetComponent<Renderer>().sharedMaterial=m;return g;}
    static GameObject Cyl(string n, Vector3 p, Vector3 s, Material m, Transform par=null){var g=GameObject.CreatePrimitive(PrimitiveType.Cylinder);g.name=n;g.transform.SetParent(par??root);g.transform.localPosition=p;g.transform.localScale=s;g.GetComponent<Renderer>().sharedMaterial=m;return g;}
    static GameObject Sphere(string n, Vector3 p, Vector3 s, Material m, Transform par=null){var g=GameObject.CreatePrimitive(PrimitiveType.Sphere);g.name=n;g.transform.SetParent(par??root);g.transform.localPosition=p;g.transform.localScale=s;g.GetComponent<Renderer>().sharedMaterial=m;return g;}
    static GameObject Empty(string n, Vector3 p, Transform par=null){var g=new GameObject(n);g.transform.SetParent(par??root);g.transform.localPosition=p;return g;}

    static void Room(string name, Vector3 c, Vector2 size, bool dark=false)
    {
        var r=Empty(name,c); Cube("Floor",Vector3.zero,new Vector3(size.x,.18f,size.y),floor,r); Cube("Ceiling",new Vector3(0,3.2f,0),new Vector3(size.x,.18f,size.y),darkMetal,r);
        float w=.18f; Cube("Wall_N",new Vector3(0,1.6f,size.y/2),new Vector3(size.x,3.2f,w),wall,r); Cube("Wall_S",new Vector3(0,1.6f,-size.y/2),new Vector3(size.x,3.2f,w),wall,r); Cube("Wall_E",new Vector3(size.x/2,1.6f,0),new Vector3(w,3.2f,size.y),wall,r); Cube("Wall_W",new Vector3(-size.x/2,1.6f,0),new Vector3(w,3.2f,size.y),wall,r);
        if(dark){ for(int x=-1;x<=1;x++){Cube("BlueLight",new Vector3(x*3,2.9f,0),new Vector3(1.2f,.06f,.12f),blueGlow,r);} }
    }
    static void BuildArchitecture()
    {
        // Layout follows the GDD: Main Lab central; Entrance west; Bathroom east; Office south; Secret behind Office; Storage/Server/Security north.
        Room("MainLab",new Vector3(0,0,0),new Vector2(12,10));
        Room("Entrance",new Vector3(-9,0,0),new Vector2(5,5));
        Room("Bathroom",new Vector3(9,0,0),new Vector2(5,5));
        Room("DrOffice",new Vector3(2,-7,0),new Vector2(7,5));
        Room("SecretRoom",new Vector3(2,-12.5f,0),new Vector2(7,5),true);
        Room("ServerRoom",new Vector3(0,7,0),new Vector2(6,4.5f),true);
        Room("Storage",new Vector3(-5.5f,7,0),new Vector2(4.5f,4.5f),true);
        Room("SecurityRoom",new Vector3(5.5f,7,0),new Vector2(4.5f,4.5f),true);
        // Door openings are represented by darker door slabs; remove/replace wall sections as final art pass.
        Door("MainEntranceDoor",new Vector3(-6.05f,1.25f,0),new Vector3(.25f,2.5f,2.0f));
        Door("OfficeDoor",new Vector3(2,1.25f,-5.05f),new Vector3(2.0f,2.5f,.25f));
        Door("BathroomDoor",new Vector3(6.05f,1.25f,0),new Vector3(.25f,2.5f,2.0f));
        Door("ServerDoor",new Vector3(0,1.25f,5.05f),new Vector3(2,2.5f,.25f));
        Door("SecurityDoor",new Vector3(5.5f,1.25f,4.75f),new Vector3(1.8f,2.5f,.25f));
        Door("StorageDoor",new Vector3(-3.25f,1.25f,7),new Vector3(.25f,2.5f,1.8f));
        Door("SecretSlidingWall",new Vector3(2,1.25f,-9.55f),new Vector3(3.0f,2.5f,.18f));
    }
    static void Door(string n,Vector3 p,Vector3 s){Cube(n,p,s,darkMetal);Cube(n+"_Handle",p+new Vector3(s.x>.5f?0.7f:.18f,0, s.z>.5f?0.7f:.18f),new Vector3(.08f,.35f,.08f),accent);}

    static void BuildRooms()
    {
        MainLab(); Office(); Storage(); Server(); Security(); Bathroom(); Secret(); Entrance();
    }
    static void Table(Vector3 p,Vector3 s){Cube("TableTop",p+Vector3.up*s.y/2,new Vector3(s.x,.18f,s.z),wood); foreach(float x in new[]{-s.x*.42f,s.x*.42f}) foreach(float z in new[]{-s.z*.42f,s.z*.42f}) Cube("Leg",p+new Vector3(x,s.y/2*.0f,z),new Vector3(.16f,s.y,.16f),darkMetal);}
    static void Chair(Vector3 p){Cyl("ChairSeat",p+new Vector3(0,.55f,0),new Vector3(.65f,.12f,.65f),black);Cube("ChairBack",p+new Vector3(0,1.15f,.28f),new Vector3(.7f,1.1f,.12f),black);Cyl("ChairStem",p+new Vector3(0,.25f,0),new Vector3(.12f,.5f,.12f),darkMetal);}
    static void Monitor(Vector3 p,Vector3 scale=new Vector3(1.3f,.85f,.08f)){Cube("Monitor",p,scale,black);Cube("Screen",p+new Vector3(0,0,-.05f),scale*.8f,blueGlow);}
    static void Cabinet(Vector3 p){Cube("Cabinet",p+Vector3.up*.9f,new Vector3(1.1f,1.8f,.65f),darkMetal);for(int i=0;i<3;i++)Cube("Drawer",p+new Vector3(0,.4f+i*.45f,-.34f),new Vector3(.9f,.35f,.05f),accent);}
    static void MainLab(){var r=GameObject.Find("MainLab").transform;Table(new Vector3(0,0,0),new Vector3(5.5f,.9f,2.1f));Chair(new Vector3(-2,0,-1.7f));Chair(new Vector3(0,0,-1.7f));Chair(new Vector3(2,0,-1.7f));Monitor(new Vector3(0,1.45f,0));Monitor(new Vector3(-1.8f,1.35f,.2f),new Vector3(.9f,.6f,.06f));Laptop(new Vector3(2,1.08f,.2f));Cabinet(new Vector3(-4,0,3.8f));Cabinet(new Vector3(4,0,3.8f));Shelf(new Vector3(-4,1.6f,2.9f));Shelf(new Vector3(4,1.6f,2.9f));Watch(new Vector3(0,1.12f,.55f));BrokenPhone(new Vector3(-2.7f,1.05f,.3f));ClueMark(new Vector3(3.0f,.015f,2.2f));}
    static void Office(){var r=GameObject.Find("DrOffice").transform;Table(new Vector3(2,-7,-.5f),new Vector3(3.5f,.75f,1.4f));Chair(new Vector3(2,-7,-1.8f));Monitor(new Vector3(2,-7+.95f,-.5f),new Vector3(1.4f,.8f,.08f));Cube("DeskLamp",new Vector3(3.1f,-7+.95f,-.2f),new Vector3(.18f,.9f,.18f),white);Cabinet(new Vector3(4.8f,-7,1.5f));Shelf(new Vector3(-.5f,-7,1.6f));Cube("StuckDrawer",new Vector3(4.6f,-7+.55f,-1.7f),new Vector3(1.2f,.6f,.8f),wood);Cube("WallPainting",new Vector3(4.9f,-7+1.7f,2.3f),new Vector3(1.8f,1.2f,.08f),accent);}
    static void Storage(){Cabinet(new Vector3(-6.7f,7,-1.2f));Cabinet(new Vector3(-4.3f,7,-1.2f));Shelf(new Vector3(-6.8f,7,1.1f));for(int i=0;i<6;i++)Cube("StorageBox",new Vector3(-7.2f+(i%3)*1.1f,.55f+((i/3)*.8f),1.0f),new Vector3(.8f,.6f,.8f),wood);Cyl("Wrench",new Vector3(-5.1f,.55f,-1.5f),new Vector3(.08f,.65f,.08f),accent);Cube("KeychainHalf_A",new Vector3(-4.6f,.45f,-1.4f),new Vector3(.45f,.06f,.18f),accent);}
    static void Server(){for(int x=-2;x<=2;x+=2) ServerRack(new Vector3(x,7,0));Cube("PowerRestorationPanel",new Vector3(2.7f,1.4f,1.8f),new Vector3(.12f,1.3f,1.2f),darkMetal);for(int i=0;i<4;i++)Cyl("PanelWire",new Vector3(2.55f,1.3f+.35f*i,1.2f),new Vector3(.05f,.05f,.5f),blueGlow);}
    static void Security(){for(int i=-2;i<=2;i++){Monitor(new Vector3(5.5f+i*1.55f,8.0f,1.9f),new Vector3(1.25f,.8f,.08f));Monitor(new Vector3(5.5f+i*1.55f,7.0f,1.9f),new Vector3(1.25f,.8f,.08f));}Table(new Vector3(5.5f,7,-1.2f),new Vector3(3.5f,.8f,1.1f));Chair(new Vector3(5.5f,7,-2.1f));}
    static void Bathroom(){var r=GameObject.Find("Bathroom").transform;Cube("Sink",new Vector3(8.0f,1.0f,1.7f),new Vector3(1.5f,.8f,.7f),white);Cube("Mirror",new Vector3(8.0f,2.0f,2.05f),new Vector3(1.5f,1.5f,.05f),glass);Cyl("Toilet",new Vector3(10,0.5f,1.7f),new Vector3(.55f,.5f,.55f),white);for(int i=0;i<2;i++)Cube("Stall",new Vector3(9+i*1.1f,1.3f,-1.2f),new Vector3(1,.05f,2.4f),wall);}
    static void Secret(){Table(new Vector3(2,-12.5f,0),new Vector3(4.5f,.8f,1.5f));Monitor(new Vector3(2,-11.55f,0),new Vector3(1.8f,1.1f,.08f));Cube("ProjectECHO_ScreenGlow",new Vector3(2,-11.5f,.2f),new Vector3(2.5f,1.4f,.05f),blueGlow);Cube("AudioRecorder",new Vector3(3.2f,-11.98f,.2f),new Vector3(.5f,.25f,.35f),darkMetal);Cube("ECHO_Documents",new Vector3(1,-11.98f,.3f),new Vector3(.8f,.03f,.5f),white);Cube("KeychainHalf_B",new Vector3(4,-11.98f,.4f),new Vector3(.45f,.06f,.18f),accent);}
    static void Entrance(){var r=GameObject.Find("Entrance").transform;Cube("EntranceDesk",new Vector3(-9,0,1),new Vector3(2.2f,.8f,1.0f),wood);Chair(new Vector3(-9,0,-.5f));Cube("Lockpick",new Vector3(-8.2f,1.0f,1),new Vector3(.7f,.08f,.08f),accent);}
    static void Shelf(Vector3 p){for(int i=0;i<4;i++)Cube("Shelf",p+new Vector3(0,i*.65f,0),new Vector3(2.4f,.08f,.5f),darkMetal);Cube("ShelfSide",p+new Vector3(-1.1f,1.0f,0),new Vector3(.08f,2.1f,.5f),darkMetal);Cube("ShelfSide",p+new Vector3(1.1f,1.0f,0),new Vector3(.08f,2.1f,.5f),darkMetal);}
    static void ServerRack(Vector3 p){Cube("ServerRack",p+Vector3.up*1.1f,new Vector3(1.0f,2.2f,.8f),darkMetal);for(int i=0;i<5;i++)Cube("ServerUnit",p+new Vector3(0,.35f+i*.35f,-.43f),new Vector3(.8f,.2f,.05f),black);for(int i=0;i<4;i++)Sphere("LED",p+new Vector3(-.3f+i*.2f,.5f,-.48f),new Vector3(.05f,.05f,.05f),blueGlow);}
    static void Laptop(Vector3 p){Cube("LaptopBase",p,new Vector3(1.2f,.08f,.75f),darkMetal);Cube("LaptopScreen",p+new Vector3(0,.4f,.32f),new Vector3(1.1f,.75f,.08f),black);Cube("LaptopDisplay",p+new Vector3(0,.4f,.27f),new Vector3(.9f,.55f,.03f),blueGlow);}
    static void Watch(Vector3 p){Cyl("StoppedWatch",p,new Vector3(.22f,.04f,.22f),white);}
    static void BrokenPhone(Vector3 p){Cube("BrokenPhone",p,new Vector3(.55f,.07f,.95f),black);}
    static void ClueMark(Vector3 p){Cube("ClueBloodMark",p,new Vector3(1.2f,.01f,.5f),blood);}
    static void BuildLighting(){foreach(var room in GameObject.Find("TRUTH_ENVIRONMENT").GetComponentsInChildren<Transform>()){if(!room.name.Contains("Room")&&!room.name.Contains("Lab")&&!room.name.Contains("Office")&&!room.name.Contains("Bathroom")&&!room.name.Contains("Entrance"))continue;var l=new GameObject(room.name+"_Light").AddComponent<Light>();l.type=LightType.Point;l.range=9;l.intensity=3;l.transform.position=room.position+Vector3.up*2.7f;if(room.name.Contains("Security")||room.name.Contains("Server")||room.name.Contains("Secret"))l.color=new Color(.15f,.35f,1f);else l.color=new Color(1f,.92f,.8f);l.transform.SetParent(root);}}
    static void BuildPreviewCamera(){var c=new GameObject("EnvironmentPreviewCamera").AddComponent<Camera>();c.transform.position=new Vector3(0,18,-24);c.transform.rotation=Quaternion.Euler(38,0,0);c.fieldOfView=55;c.transform.SetParent(root);}
}
