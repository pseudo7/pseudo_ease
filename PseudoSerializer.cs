using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable]
public class CartData
{
    public string productName;
    public string productDetails;
    public int quantity;
    public string price;
    public string pid;
    public string discount;
    public string spriteEnum;
    public byte[] imageSprite;
    public string printData;
    public string productCode;

    public CartData(string productName, string productDetails, int quantity, string price, string pid, string discount, string spriteEnum, byte[] imageSprite, string printData, string productCode)
    {
        this.productName = productName;
        this.productDetails = productDetails;
        this.quantity = quantity;
        this.price = price;
        this.pid = pid;
        this.discount = discount;
        this.spriteEnum = spriteEnum;
        this.imageSprite = imageSprite;
        this.printData = printData;
        this.productCode = productCode;
    }

    public override string ToString()
    {
        return string.Format("ProdName: {0}, ProdDetails: {1}, Qauntity: {2}, Price: {3}, PID: {4}, Discount: {5}, SpriteEnum: {6},\nPrintData: {7},\nProductCode: {8}", productName, productDetails, quantity, price, pid, discount, spriteEnum, printData, productCode);
    }
}

[Serializable]
public class ListCartData
{
    public List<CartData> savedCartData;

    public ListCartData(List<CartData> list)
    {
        savedCartData = list;
    }
}

public enum CartUpdateTypes
{
    ADD, REMOVE, ADD_ALL, REMOVE_ALL
}


public class PseudoSerializer : MonoBehaviour
{
    public const string CART_ITEM_KEY = "CART_ITEM_KEY";

    public static PseudoSerializer Instance;

    public Transform cartContent;
    public GameObject cartPrefab10, cartPrefab8;

    public CartCountManager[] cartCountManagers;

    List<CartData> list = new List<CartData>();

    string ProductName; string ProductDetails; string Price; string PID; int Quantity; string Discount; string SpriteEnum; byte[] ImageSprite; string PrintData; string ProductCode;


    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        LoadCartItems();
    }

    public void LoadCartItems()
    {
        ListCartData loadedData;

        if (PlayerPrefs.HasKey(CART_ITEM_KEY))
            loadedData = JsonUtility.FromJson<ListCartData>(PlayerPrefs.GetString(CART_ITEM_KEY));
        else
        {
            Debug.LogError("<color=red>No Key Found</color>");
            loadedData = new ListCartData(new List<CartData>() { });
            PlayerPrefs.SetString(CART_ITEM_KEY, JsonUtility.ToJson(loadedData));
            PlayerPrefs.Save();
        }

        if (loadedData.savedCartData.Count < 1)
        {
            Debug.LogError("<color=red>No Saved Found</color>");
            return;
        }


        foreach (CartData data in loadedData.savedCartData)
        {
            GameObject newPrefab = Instantiate((PseudoLevelLoader.IsTabletResolutionWide ? cartPrefab10 : cartPrefab8));


            newPrefab.transform.GetChild(0).GetComponent<Image>().sprite = GetSpriteFromByte(data.imageSprite);
            newPrefab.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = data.productName;
            newPrefab.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = data.productDetails;
            newPrefab.transform.GetChild(2).GetChild(4).GetComponent<Text>().text = int.Parse(data.price) <= 0 ? "4660" : data.price;
            newPrefab.transform.GetChild(1).GetChild(3).GetComponent<Text>().text = data.pid;
            newPrefab.transform.GetChild(1).GetChild(4).GetComponent<Text>().text = data.productCode;
            newPrefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text = data.quantity.ToString();
            newPrefab.transform.GetChild(5).GetComponent<Text>().text = data.discount;
            newPrefab.transform.GetChild(6).GetComponent<Text>().text = data.spriteEnum;
            newPrefab.transform.GetChild(7).GetComponent<Text>().text = data.printData;

            newPrefab.transform.parent = cartContent.transform;
            newPrefab.SetActive(true);

            Debug.LogErrorFormat("<color=green>Saved Data Item: {0}</color>", data);
        }
    }



     public void ClearCartItems()
    {
        for (int i = 0; i < cartContent.childCount; i++)
            Destroy(cartContent.GetChild(i).gameObject);
    }

    public void Add()
    {
        int cartCount = -100;

        Transform chidlTransform = cartContent.GetChild(cartContent.childCount - 1);

        ImageSprite = GetByteFromSprite(chidlTransform.GetChild(0).GetComponent<Image>().sprite);
        ProductName = chidlTransform.GetChild(1).GetChild(0).GetComponent<Text>().text;// = sofaStyleText.text;  //sofa name
        ProductDetails = chidlTransform.GetChild(1).GetChild(1).GetComponent<Text>().text;// = "Fabric : " + sofaFabricText.text + ", Color : " + sofaColorText.text + ", Seating : " + sofaSeaterText.text;
        Price = chidlTransform.GetChild(2).GetChild(4).GetComponent<Text>().text;// = sofaPriceText.text;  //price
        PID = chidlTransform.GetChild(1).GetChild(3).GetComponent<Text>().text;// = sofaProductId.text;  //product id
        Quantity = int.Parse((PseudoLevelLoader.IsTabletResolutionWide ? cartPrefab10 : cartPrefab8).transform.GetChild(2).GetChild(3).GetComponent<Text>().text);
        Discount = (PseudoLevelLoader.IsTabletResolutionWide ? cartPrefab10 : cartPrefab8).transform.GetChild(5).GetComponent<Text>().text;
        SpriteEnum = (PseudoLevelLoader.IsTabletResolutionWide ? cartPrefab10 : cartPrefab8).transform.GetChild(6).GetComponent<Text>().text;
        PrintData = chidlTransform.GetChild(7).GetComponent<Text>().text;
        ProductCode = chidlTransform.GetChild(1).GetChild(4).GetComponent<Text>().text;

        ListCartData loadedData;

        if (PlayerPrefs.HasKey(CART_ITEM_KEY))
        {
            loadedData = JsonUtility.FromJson<ListCartData>(PlayerPrefs.GetString(CART_ITEM_KEY));
            loadedData.savedCartData.Add(new CartData(ProductName, ProductDetails, Quantity, Price, PID, Discount, SpriteEnum, ImageSprite, PrintData, ProductCode));
            cartCount = loadedData.savedCartData.Count;
            PlayerPrefs.SetString(CART_ITEM_KEY, JsonUtility.ToJson(loadedData));

        }
        else
        {
            List<CartData> newList = new List<CartData>() { new CartData(ProductName, ProductDetails, Quantity, Price, PID, Discount, SpriteEnum, ImageSprite, PrintData, ProductCode) };

            loadedData = new ListCartData(newList);
            cartCount = 1;
            PlayerPrefs.SetString(CART_ITEM_KEY, JsonUtility.ToJson(loadedData));
        }

        UpdateCartCount(cartCount);

        PlayerPrefs.Save();
    }

    public void AddAll()
    {
        for (int i = 0; i < cartContent.childCount; i++)
        {
            Transform chidlTransform = cartContent.GetChild(i);

            ImageSprite = GetByteFromSprite(chidlTransform.GetChild(0).GetComponent<Image>().sprite);
            ProductName = chidlTransform.GetChild(1).GetChild(0).GetComponent<Text>().text;// = sofaStyleText.text;  //sofa name
            ProductDetails = chidlTransform.GetChild(1).GetChild(1).GetComponent<Text>().text;// = "Fabric : " + sofaFabricText.text + ", Color : " + sofaColorText.text + ", Seating : " + sofaSeaterText.text;
            Price = chidlTransform.GetChild(2).GetChild(4).GetComponent<Text>().text;// = sofaPriceText.text;  //price
            PID = chidlTransform.GetChild(1).GetChild(3).GetComponent<Text>().text;// = sofaProductId.text;  //product id
            Quantity = int.Parse((PseudoLevelLoader.IsTabletResolutionWide ? cartPrefab10 : cartPrefab8).transform.GetChild(2).GetChild(3).GetComponent<Text>().text);
            Discount = (PseudoLevelLoader.IsTabletResolutionWide ? cartPrefab10 : cartPrefab8).transform.GetChild(5).GetComponent<Text>().text;
            SpriteEnum = (PseudoLevelLoader.IsTabletResolutionWide ? cartPrefab10 : cartPrefab8).transform.GetChild(6).GetComponent<Text>().text;

            ListCartData loadedData;

            if (PlayerPrefs.HasKey(CART_ITEM_KEY))
            {
                loadedData = JsonUtility.FromJson<ListCartData>(PlayerPrefs.GetString(CART_ITEM_KEY));
                loadedData.savedCartData.Add(new CartData(ProductName, ProductDetails, Quantity, Price, PID, Discount, SpriteEnum, ImageSprite, PrintData, ProductCode));

                PlayerPrefs.SetString(CART_ITEM_KEY, JsonUtility.ToJson(loadedData));

            }
            else
            {
                List<CartData> newList = new List<CartData>() { new CartData(ProductName, ProductDetails, Quantity, Price, PID, Discount, SpriteEnum, ImageSprite, PrintData, ProductCode) };

                loadedData = new ListCartData(newList);
                PlayerPrefs.SetString(CART_ITEM_KEY, JsonUtility.ToJson(loadedData));
            }
            PlayerPrefs.Save();
        }
    }

    public void Remove(Transform itemTransform)
    {
        int removeIndex = itemTransform.parent.GetSiblingIndex();

        ListCartData loadedData = JsonUtility.FromJson<ListCartData>(PlayerPrefs.GetString(CART_ITEM_KEY));
        loadedData.savedCartData.RemoveAt(removeIndex);
        PlayerPrefs.SetString(CART_ITEM_KEY, JsonUtility.ToJson(loadedData));
        PlayerPrefs.Save();

        UpdateCartCount(loadedData.savedCartData.Count);

    }

    public void RemoveAll()
    {
        ListCartData loadedData = new ListCartData(new List<CartData>() { });
        PlayerPrefs.SetString(CART_ITEM_KEY, JsonUtility.ToJson(loadedData));
        PlayerPrefs.Save();
        UpdateCartCount(0);
        ClearCartItems();
    }

    public Sprite GetSpriteFromByte(byte[] spriteData)
    {
        Texture2D texture2D = new Texture2D(1150, 750, TextureFormat.ARGB32, false);
        texture2D.LoadImage(spriteData);
        texture2D.Apply();

        return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(.5f, .5f));
    }

    public byte[] GetByteFromSprite(Sprite sprite)
    {
        if (sprite.name.StartsWith("dining", StringComparison.InvariantCultureIgnoreCase))
            return sprite.texture.EncodeToJPG(100);
        return sprite.texture.EncodeToJPG(30);
    }

    public void UpdateCartCount(int count)
    {
        foreach (CartCountManager cartCountManager in cartCountManagers)
            cartCountManager.SetCartCount(count);
    }
}
