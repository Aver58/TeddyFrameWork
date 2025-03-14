using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.EasyMinimapSystem
{
    public class Constructor : MonoBehaviour
    {
        //Private variables
        private Vector3 startingPosition = Vector3.zero;
        private bool gettedFirstPos = false;

        //Public variables        
        public Sprite playerIndicator;
        public Rigidbody playerRigidbody;
        public Transform destinationCube;
        public Sprite rendererBaseShape;
        public Sprite rendererNorth;
        public GameObject reservedForRenderer;
        public Texture2D compassTexture;
        public Font compassFont;
        public GameObject reservedForCompass;
        public GameObject reservedForScanner;
        public MinimapRoutes minimapRoutes;
        public GameObject reservedForText;
        public GameObject reservedForLineA;
        public GameObject reservedForLineB;

        // Start is called before the first frame update
        void Start()
        {
            //Construct all minimap itens with delay
            StartCoroutine(ConstructAllMinimapComponents());
        }

        void Update()
        {
            //This is only for gameplay, not consider this void
            if (gettedFirstPos == false)
                startingPosition = playerRigidbody.transform.position;

            if (playerRigidbody.position.y <= -10.0f)
            {
                playerRigidbody.transform.position = startingPosition;
                playerRigidbody.transform.eulerAngles = Vector3.zero;
            }

            destinationCube.Rotate(0, 250 * Time.deltaTime, 0);

            gettedFirstPos = true;
        }

        IEnumerator ConstructAllMinimapComponents()
        {
            yield return null;

            //Create the minimap camera
            MinimapCamera playerCamera = playerRigidbody.gameObject.AddComponent<MinimapCamera>();
            playerCamera.fieldOfView = 60;

            yield return new WaitForSeconds(1.5f);

            //Create the minimap renderer
            MinimapRenderer mRenderer = reservedForRenderer.AddComponent<MinimapRenderer>();
            mRenderer.minimapCameraToShow = playerCamera;
            mRenderer.baseShapeSprite = rendererBaseShape;
            mRenderer.showNorthIcon = true;
            mRenderer.northIconSprite = rendererNorth;

            yield return new WaitForSeconds(3f);

            //Create the player item
            MinimapItem playerItem = playerRigidbody.transform.gameObject.AddComponent<MinimapItem>();
            playerItem.itemSprite = playerIndicator;
            playerItem.sizeOnMinimap = new Vector3(14, 0, 14);

            //Create the destination item
            MinimapItem destinationItem = destinationCube.gameObject.AddComponent<MinimapItem>();
            destinationItem.itemSprite = rendererBaseShape;
            destinationItem.spriteColor = Color.yellow;
            destinationItem.sizeOnMinimap = new Vector3(14, 0, 14);
            destinationItem.particlesHighlightMode = MinimapItem.ParticlesHighlightMode.WavesIncrease;
            destinationItem.particlesSprite = rendererBaseShape;
            destinationItem.particlesColor = new Color(255f, 255f, 255f, 0.3f);

            yield return new WaitForSeconds(3f);

            //Create the minimap compass
            MinimapCompass mCompass = reservedForCompass.AddComponent<MinimapCompass>();
            mCompass.targetPlayerTransform = playerRigidbody.transform;
            mCompass.compassTexture = compassTexture;
            mCompass.showDegreesToNorth = true;
            mCompass.degreesFont = compassFont;

            yield return new WaitForSeconds(3f);

            //Start showing routes
            minimapRoutes.startingPoint = playerRigidbody.transform;
            minimapRoutes.destinationPoint = destinationCube;
            minimapRoutes.lineWidthSize = 4.0f;
            minimapRoutes.lineRenderColor = Color.red;
            minimapRoutes.StartCalculatingAndShowRotesToDestination();

            //Create the minimap scanner (varios minimap scanners to scan entire scene using a loop)
            MinimapScanner mScanner = reservedForScanner.AddComponent<MinimapScanner>();
            mScanner.scanArea = MinimapScanner.ScanArea.Units60;
            //mScanner.gameObjectsToIgnore.Add(playerRigidbody.transform.gameObject);
            mScanner.gameObjectsToIgnore.Add(destinationCube.transform.gameObject);
            mScanner.DoScanInThisAreaOfComponentAndShowOnMinimap();
            yield return new WaitForSeconds(1.0f);
            MinimapScanner created = mScanner.CreateNewMinimapScannerBeside(MinimapScanner.CreateScanInSide.Back);
            created.DoScanInThisAreaOfComponentAndShowOnMinimap();
            yield return new WaitForSeconds(1.0f);
            MinimapScanner lastCreated = mScanner;
            for (int i = 0; i < 4; i++)
            {
                MinimapScanner createdRightSide = lastCreated.CreateNewMinimapScannerBeside(MinimapScanner.CreateScanInSide.Right);
                createdRightSide.DoScanInThisAreaOfComponentAndShowOnMinimap();
                yield return new WaitForSeconds(1.0f);
                MinimapScanner createdBack = createdRightSide.CreateNewMinimapScannerBeside(MinimapScanner.CreateScanInSide.Back);
                createdBack.DoScanInThisAreaOfComponentAndShowOnMinimap();
                lastCreated = createdRightSide;
                yield return new WaitForSeconds(1.0f);
            }

            yield return new WaitForSeconds(1f);

            //Add the destination item to be highlighted
            mCompass.AddMinimapItemToBeHighlighted(destinationItem);
            mRenderer.AddMinimapItemToBeHighlighted(destinationItem);

            yield return new WaitForSeconds(1f);

            //Add the destination item to be highlighted
            mCompass.RemoveMinimapItemOfHighlight(destinationItem);
            mRenderer.RemoveMinimapItemOfHighlight(destinationItem);

            yield return new WaitForSeconds(1f);

            //Add the destination item to be highlighted
            mCompass.AddMinimapItemToBeHighlighted(destinationItem);
            mRenderer.AddMinimapItemToBeHighlighted(destinationItem);

            yield return new WaitForSeconds(1f);

            //Create the minimap text
            MinimapText minimapText = reservedForText.AddComponent<MinimapText>();
            minimapText.textToRender = "Runtime Scene";
            minimapText.fontSize = 110;

            yield return new WaitForSeconds(1f);

            //Create the minimap line
            MinimapLine minimapLine = reservedForLineA.AddComponent<MinimapLine>();
            minimapLine.SetNewWorldPositions(new Vector3[] { reservedForLineA.transform.position, reservedForLineB.transform.position });
            minimapLine.lineWidthSize = 4.0f;
            minimapLine.lineRenderColor = Color.green;

            Debug.Log("The scene is ready! All Minimap Components was added!");
        }
    }
}