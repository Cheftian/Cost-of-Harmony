using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 1.0f; // Jarak Elias harus berada untuk berinteraksi dengan objek biasa
    public float roomTriggerStopDistance = 0.5f; // Jarak Elias berhenti dari RoomTrigger
    public float dragRadius = 2.0f; // Radius maksimal objek bisa digerakkan dari Elias
    public LayerMask interactableLayer; // Layer untuk objek yang bisa diinteraksikan (selain HanoiDisk)
    public LayerMask roomTriggerLayer; // Layer untuk RoomTrigger
    public LayerMask hanoiDiskLayer; // BARU: Layer untuk HanoiDisk
    public LayerMask hanoiPegLayer; // BARU: Layer untuk HanoiPeg

    private PlayerMovement playerMovement;
    private InteractableObject currentInteractable; // Ini bisa jadi InteractableObject biasa atau HanoiDisk
    private RoomTrigger currentRoomTrigger;
    private GameObject dragDistanceIndicator;
    private bool isDragging = false;
    private bool awaitingInteractionPoint = false; 

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement script not found on Elias!");
            enabled = false;
            return;
        }
        CreateDragDistanceIndicator();
    }

    void Update()
    {
        if (!isDragging && Input.GetMouseButtonDown(0))
        {
            HandleInteractionClick();
        }
        else if (isDragging)
        {
            HandleDragging();
        }
    }

    void HandleInteractionClick()
    {
        Vector2 mouseClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseClickPos, Vector2.zero, Mathf.Infinity);

        Debug.Log($"Klik Terdeteksi di: {mouseClickPos}");

        // Reset current references and movement states for a new click
        currentInteractable = null;
        currentRoomTrigger = null;
        awaitingInteractionPoint = false;

        if (hit.collider != null)
        {
            Debug.Log($"Klik Mengenai: {hit.collider.gameObject.name} (Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)})");

            // Cek apakah yang diklik adalah RoomTrigger
            if (((1 << hit.collider.gameObject.layer) & roomTriggerLayer) != 0)
            {
                currentRoomTrigger = hit.collider.GetComponent<RoomTrigger>();
                if (currentRoomTrigger != null)
                {
                    currentRoomTrigger.SetTriggerClicked(); 
                    
                    awaitingInteractionPoint = true; 
                    playerMovement.SetTargetPosition(currentRoomTrigger.transform.position.x, roomTriggerStopDistance);
                    Debug.Log($"[PlayerInteraction] Mengklik RoomTrigger: {currentRoomTrigger.name}. Elias bergerak ke sana.");
                    return; 
                }
            }
            // --- BARU: Cek apakah yang diklik adalah HanoiDisk ---
            else if (((1 << hit.collider.gameObject.layer) & hanoiDiskLayer) != 0)
            {
                HanoiDisk clickedDisk = hit.collider.GetComponent<HanoiDisk>();
                if (clickedDisk != null)
                {
                    // Aturan Tower of Hanoi: Hanya disk teratas di peg yang bisa diambil
                    HanoiPeg parentPeg = null;
                    if (clickedDisk.currentStackPoint != null) // Jika disk berada di peg
                    {
                        parentPeg = clickedDisk.currentStackPoint.GetComponent<HanoiPeg>();
                    }

                    if (parentPeg != null && parentPeg.topDisk != clickedDisk)
                    {
                        Debug.LogWarning($"Disk {clickedDisk.name} tidak bisa diambil. Bukan disk teratas di peg {parentPeg.name}.");
                        return; // Tidak bisa mengambil disk jika bukan yang teratas
                    }
                    
                    currentInteractable = clickedDisk; // Treat HanoiDisk as currentInteractable
                    
                    float distanceToInteractionPoint = Vector2.Distance(transform.position, currentInteractable.GetInteractionPoint());

                    if (distanceToInteractionPoint <= interactionDistance + playerMovement.stopDistance)
                    {
                        isDragging = true;
                        playerMovement.CanMove(false);
                        currentInteractable.StartDrag(); // Panggil StartDrag dari HanoiDisk (melalui InteractableObject base)
                        
                        // Hapus disk dari peg asalnya saat mulai drag
                        if (parentPeg != null)
                        {
                            parentPeg.RemoveDisk(clickedDisk);
                        }

                        if (dragDistanceIndicator != null) dragDistanceIndicator.SetActive(true);
                        Debug.Log($"[PlayerInteraction] Elias sudah cukup dekat dengan Disk {currentInteractable.name}. Langsung mulai drag.");
                    }
                    else
                    {
                        awaitingInteractionPoint = true;
                        float targetX = clickedDisk.GetInteractionPoint().x; // Gunakan clickedDisk
                        float playerX = transform.position.x;
                        
                        if (playerX < targetX)
                        {
                            targetX -= interactionDistance;
                        }
                        else
                        {
                            targetX += interactionDistance;
                        }

                        playerMovement.SetTargetPosition(targetX, playerMovement.stopDistance);
                        Debug.Log($"[PlayerInteraction] Elias bergerak ke Disk {currentInteractable.name} untuk interaksi.");
                    }
                }
            }
            // --- AKHIR BARU: Cek HanoiDisk ---

            // Jika bukan RoomTrigger atau HanoiDisk, cek apakah itu InteractableObject biasa
            else if (((1 << hit.collider.gameObject.layer) & interactableLayer) != 0)
            {
                currentInteractable = hit.collider.GetComponent<InteractableObject>();
                if (currentInteractable != null)
                {
                    // Pastikan objek ini BUKAN HanoiDisk (karena sudah ditangani di atas)
                    if (currentInteractable is HanoiDisk) {
                        currentInteractable = null; // Reset, karena sudah ditangani di blok HanoiDisk
                        return; 
                    }

                    float distanceToInteractionPoint = Vector2.Distance(transform.position, currentInteractable.GetInteractionPoint());

                    if (distanceToInteractionPoint <= interactionDistance + playerMovement.stopDistance)
                    {
                        isDragging = true;
                        playerMovement.CanMove(false);
                        currentInteractable.StartDrag();
                        if (dragDistanceIndicator != null) dragDistanceIndicator.SetActive(true);
                        Debug.Log($"[PlayerInteraction] Elias sudah cukup dekat dengan {currentInteractable.name}. Langsung mulai drag.");
                    }
                    else
                    {
                        awaitingInteractionPoint = true;
                        float targetX = currentInteractable.GetInteractionPoint().x; 
                        float playerX = transform.position.x;
                        
                        if (playerX < targetX)
                        {
                            targetX -= interactionDistance;
                        }
                        else
                        {
                            targetX += interactionDistance;
                        }

                        playerMovement.SetTargetPosition(targetX, playerMovement.stopDistance);
                        Debug.Log($"[PlayerInteraction] Elias bergerak ke {currentInteractable.name} untuk interaksi.");
                    }
                }
            }
            else
            {
                Debug.Log($"[PlayerInteraction] Klik pada objek non-interactable/RoomTrigger/HanoiDisk: {hit.collider.gameObject.name}");
            }
        }
        else
        {
            Debug.Log("[PlayerInteraction] Tidak ada objek yang diklik.");
        }
    }

    void HandleDragging()
    {
        if (currentInteractable == null) return;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 clampedDragPos = mouseWorldPos;
        Vector2 playerPos = transform.position;

        Vector2 directionToMouse = mouseWorldPos - playerPos;

        if (directionToMouse.magnitude > dragRadius)
        {
            clampedDragPos = playerPos + directionToMouse.normalized * dragRadius;
        }

        currentInteractable.DragTo(clampedDragPos);

        if (dragDistanceIndicator != null)
        {
            dragDistanceIndicator.transform.position = playerPos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            TryDropObject(); 
        }
    }

    void TryDropObject()
    {
        if (currentInteractable == null) return;

        // Coba cast ke HanoiDisk
        HanoiDisk diskToDrop = currentInteractable as HanoiDisk;

        if (diskToDrop != null) // Jika objek yang di-drag adalah HanoiDisk
        {
            // Raycast ke bawah disk untuk mencari HanoiPeg
            RaycastHit2D hit = Physics2D.Raycast(diskToDrop.transform.position, Vector2.down, 
                                                diskToDrop.GetComponent<Collider2D>().bounds.extents.y + 0.1f, 
                                                hanoiPegLayer); 
            HanoiPeg targetPeg = null;

            if (hit.collider != null)
            {
                targetPeg = hit.collider.GetComponent<HanoiPeg>();
                Debug.Log($"Raycast drop mengenai: {hit.collider.gameObject.name} (Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)})");
            }

            if (targetPeg != null && targetPeg.CanPlaceDisk(diskToDrop)) // Panggil CanPlaceDisk dari HanoiPeg
            {
                targetPeg.PlaceDisk(diskToDrop); // Panggil PlaceDisk dari HanoiPeg
                Debug.Log($"Disk {diskToDrop.name} berhasil diletakkan di Peg {targetPeg.name}");
            }
            else
            {
                // Tidak bisa diletakkan di HanoiPeg atau tidak ada Peg yang valid, drop biasa
                Debug.Log($"Disk {diskToDrop.name} di-drop secara bebas (atau tidak valid di Peg).");
            }
        }
        else // Jika bukan HanoiDisk, drop seperti InteractableObject biasa
        {
            Debug.Log($"Objek {currentInteractable.name} di-drop secara bebas.");
        }

        StopDragging(); 
    }

    public void OnReachedInteractionPoint()
    {
        Debug.Log("[PlayerInteraction] OnReachedInteractionPoint dipanggil. AwaitingInteractionPoint: " + awaitingInteractionPoint);
        if (awaitingInteractionPoint)
        {
            awaitingInteractionPoint = false; 

            // Jika target adalah RoomTrigger
            if (currentRoomTrigger != null)
            {
                Debug.Log($"[PlayerInteraction] Elias mencapai posisi RoomTrigger: {currentRoomTrigger.name}.");
                currentRoomTrigger = null; 
                playerMovement.CanMove(true); 
            }
            // Jika target adalah InteractableObject (termasuk HanoiDisk)
            else if (currentInteractable != null)
            {
                isDragging = true;
                playerMovement.CanMove(false); 
                currentInteractable.StartDrag();
                
                // BARU: Hapus dari peg jika ini adalah HanoiDisk dan sedang dipeg (jika baru pindah posisi tapi belum di-drop)
                HanoiDisk diskToDrag = currentInteractable as HanoiDisk;
                if (diskToDrag != null && diskToDrag.currentStackPoint != null)
                {
                    diskToDrag.currentStackPoint.GetComponent<HanoiPeg>().RemoveDisk(diskToDrag); 
                }

                if (dragDistanceIndicator != null) dragDistanceIndicator.SetActive(true);
                Debug.Log($"[PlayerInteraction] Elias telah mencapai jarak interaksi dengan {currentInteractable.name}. Memulai drag.");
            }
            else
            {
                Debug.LogWarning("[PlayerInteraction] OnReachedInteractionPoint dipanggil tapi tidak ada currentRoomTrigger atau currentInteractable yang valid. Resetting movement.");
                playerMovement.CanMove(true); 
            }
        }
    }

    void StopDragging()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Drop(); 
            currentInteractable = null;
        }
        isDragging = false;
        playerMovement.CanMove(true);
        if (dragDistanceIndicator != null) dragDistanceIndicator.SetActive(false);
        Debug.Log("[PlayerInteraction] Berhenti dragging.");
    }

    public bool IsAwaitingInteraction()
    {
        return awaitingInteractionPoint;
    }

    void CreateDragDistanceIndicator()
    {
        dragDistanceIndicator = new GameObject("DragDistanceIndicator");
        SpriteRenderer indicatorRenderer = dragDistanceIndicator.AddComponent<SpriteRenderer>();
        
        Texture2D tex = new Texture2D(100, 100);
        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                Vector2 center = new Vector2(tex.width / 2, tex.height / 2);
                float dist = Vector2.Distance(new Vector2(i, j), center);
                if (dist > tex.width / 2 - 2 && dist < tex.width / 2)
                    tex.SetPixel(i, j, new Color(1, 1, 1, 0.5f));
                else
                    tex.SetPixel(i, j, new Color(0, 0, 0, 0));
            }
        }
        tex.Apply();
        indicatorRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), tex.width / (dragRadius * 2));
        dragDistanceIndicator.transform.localScale = Vector3.one * (dragRadius * 2);

        indicatorRenderer.sortingOrder = -1;
        dragDistanceIndicator.SetActive(false);
    }
}