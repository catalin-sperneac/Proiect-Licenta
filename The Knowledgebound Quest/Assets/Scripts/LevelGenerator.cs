using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private LinearRoom startingRoom;

    [SerializeField] private LinearRoom[] normalRooms;
    [SerializeField] private LinearRoom[] chestRooms;
    [SerializeField] private LinearRoom[] shopRooms;
    [SerializeField] private LinearRoom[] upgradeRooms;
    [SerializeField] private LinearRoom[] bossRooms;
    [SerializeField] private BranchRoom[] branchRooms;
    [SerializeField] private LinearRoom[] endRooms;

    [SerializeField] private int branchMinDistance = 3;

    private List<GameObject> spawned = new List<GameObject>();
    private int roomsSinceLastBranch = int.MaxValue;
    
    private Quaternion lastRotation = Quaternion.identity;

    private void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        foreach (var g in spawned)
        {
            if (g != null)
            {
                Destroy(g);
            }
        }
        spawned.Clear();

        roomsSinceLastBranch = int.MaxValue;

        Transform attachPoint = startingRoom.nextSpawnPoint;

        List<Room> roomList = BuildRoomList();

        foreach (var roomPrefab in roomList)
        {
            if (roomPrefab is BranchRoom br)
            {
                attachPoint = SpawnBranchRoom(br, attachPoint);
            }
            else if (roomPrefab is LinearRoom lr)
            {
                attachPoint = SpawnLinearRoom(lr, attachPoint);
            }
        }

        LinearRoom endRoom = SafeGetRandom(endRooms);
        if (endRoom != null)
        {
            SpawnLinearRoom(endRoom, attachPoint);
        }
    }

    private List<Room> BuildRoomList()
    {
        List<Room> list = new List<Room>();
        int currentLevel = PlayerDataManager.instance.stats.currentLevel;

        int normalCount = 10 + (currentLevel - 1);
        int bossCount = 1 + ((currentLevel - 1) / 5);
        int chestCount = ((currentLevel % 2 == 1) ? 1 : 0) + ((currentLevel - 1) / 10);
        int shopCount = ((currentLevel % 2 == 0) ? 1 : 0) + ((currentLevel - 1) / 10);
        int upgradeCount = ((currentLevel % 5 == 0) ? 1 : 0);

        for (int i = 0; i < normalCount; i++)
        {
            list.Add(SafeGetRandom(normalRooms));
        }

        for (int i = 0; i < chestCount; i++)
        {
            list.Add(SafeGetRandom(chestRooms));
        }

        for (int i = 0; i < shopCount; i++)
        {
            list.Add(SafeGetRandom(shopRooms));
        }

        for (int i = 0; i < upgradeCount; i++)
        {
            list.Add(SafeGetRandom(upgradeRooms));
        }

        for (int i = 0; i < bossCount; i++)
        {
            list.Add(SafeGetRandom(bossRooms));
        }

        Shuffle(list);

        InsertBranchRooms(list);

        return list;
    }

    private void InsertBranchRooms(List<Room> list)
    {
        if (branchRooms == null || branchRooms.Length == 0 || list.Count < branchMinDistance)
        {
            return;
        }

        int lastBranchIndex = -branchMinDistance;

        int maxBranches = list.Count / branchMinDistance;
        int branchInserted = 0;

        while (branchInserted < maxBranches)
        {
            int minIndex = lastBranchIndex + branchMinDistance;
            int maxIndex = list.Count - 1;
            if (minIndex >= maxIndex)
            {
                break;
            }

            int insertIndex = Random.Range(minIndex, maxIndex);
            list.Insert(insertIndex, SafeGetRandom(branchRooms));

            lastBranchIndex = insertIndex;
            branchInserted++;
        }
    }

    private Transform SpawnLinearRoom(LinearRoom prefab, Transform attachTo)
    {
        if (prefab == null || attachTo == null)
        {
            return attachTo;
        }

        GameObject go = Instantiate(prefab.gameObject);
        spawned.Add(go);
        LinearRoom room = go.GetComponent<LinearRoom>();

        room.transform.rotation = lastRotation;
        AlignRoomStartToAttach(room.startingPoint, attachTo, room.transform);

        roomsSinceLastBranch++;

        return room.nextSpawnPoint;
    }

    private Transform SpawnBranchRoom(BranchRoom prefab, Transform attachTo)
    {
        if (prefab == null || attachTo == null)
        { 
            return attachTo;
        }

        GameObject go = Instantiate(prefab.gameObject);
        spawned.Add(go);
        BranchRoom room = go.GetComponent<BranchRoom>();

        room.transform.rotation = lastRotation;
        AlignRoomStartToAttach(room.startingPoint, attachTo, room.transform);

        int r = Random.Range(0, 3);
        if (r == 0) //Left
        {
            lastRotation *= Quaternion.Euler(0, -90, 0);
        }
        else if (r == 1) // Forward
        {
            lastRotation *= Quaternion.Euler(0, 0, 0);
        }
        else // Right
        {
            lastRotation *= Quaternion.Euler(0, 90, 0);
        }

        roomsSinceLastBranch = 0;

        if (r == 0)
        {
            return room.leftSpawnPoint;
        }
        if (r == 2)
        {
            return room.rightSpawnPoint;
        }
        return room.forwardSpawnPoint;

    }

    private void AlignRoomStartToAttach(Transform roomStart, Transform attachTo, Transform roomRoot)
    {
        if (roomStart == null || attachTo == null || roomRoot == null)
        {
            return;
        }
        Vector3 delta = attachTo.position - roomStart.position;
        roomRoot.position += delta;
    }

    private T SafeGetRandom<T>(T[] arr) where T : class
    {
        if (arr == null || arr.Length == 0)
        {
            return null;
        }
        return arr[Random.Range(0, arr.Length)];
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
