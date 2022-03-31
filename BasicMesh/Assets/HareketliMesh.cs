using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter),typeof(MeshCollider))]
public class HareketliMesh : MonoBehaviour
{
    Vector3[] vertices;
    int[] triangles;

    Mesh mesh;
    MeshCollider meshCollider;

    public float kareBoyutu = 3;
    public int kareSayisi = 15;

    /*
    Her bir vertex'in y�kseklik yani Y eksenindeki de�erlerini
    tutaca��m�z ve hesaplay�p g�ncelleyece�imiz dizi.
    */
    float[,] yukseklik;

    //Dersten biraz farkl� olarak b�yle bir �ey de�i�ken de ekledim.
    //Bu de�i�ken sayesinde PerlinNoise derecesini ayarlayabiliyoruz.
    [Range(0, 1)]
    [SerializeField]
    float perlinScale = .75f;

    [Range(1, 15)]
    [SerializeField]
    float maksYukseklik = 7f;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
        YukseklikVerisi();
        PlaneVerisiOlustur();
        MeshOlustur();
    }

    void Update()
    {
        YukseklikVerisi();
        PlaneVerisiOlustur();
        MeshOlustur();
    }

    //Plane(D�z Zemin) Mesh Objemizi Olu�turdu�umuz Fonksiyon
    public void PlaneVerisiOlustur()
    {

        /*
        Burada kare say�s�n� bir fazla kullanma sebebimizi
        derslerde daha ayr�nt�l� anlatm��t�m.
        Ancak basit�e bahsedersek,
        5x5 �eklinde bir grid olu�turursak,
        her bir sat�r ve s�tunda 6 tane noktadan(�izgilerin kesi�imi)
        oldu�unu g�r�r�z. 
        Yani kare say�m�zdan 1 fazla
        */
        vertices = new Vector3[(kareSayisi + 1) * (kareSayisi + 1)];
        /*
        Her bir karemizin 6 tane vertexten olu�mas�n� istiyoruz.
        Bu sayede ayr�k ��genler dersimizdeki yap�y� kullanaca��z.
        Triangle i�erisinde de olu�turaca��m�z mesh yarat�rken 
        olu�turdu�umuz ��genlerin noktalar� veriyoruz.
        Her bir ��gen 3 noktadan olu�uyor, 
        ve biz 2 ��geni 1 kare olarak g�r�yoruz.
        Yani ka� kare kullanacaksak bunun i�in 6 adet de
        vertex de�erini triangles dizisinde tutaca��z.
        Bu y�zden toplam kare say�m�z� 6 ile �arp�yoruz.
        */
        triangles = new int[kareSayisi * kareSayisi * 6];

        /*
        Bir kareyi olu�tururken vertexlerin konumunu
        kare merkezinden yatay ve dikeyde istedi�imiz boyutun yar�s�
        kadar olacakt�r. Bu y�zden .5f ile 
        yar�s�n� bir de�i�kende tutuyoruz.
        */
        float vertexAralik = kareBoyutu * .5f;

        //Vertexleri olu�turmak ve dola�mak i�in kullanaca��m�z index de�i�keni
        int v = 0;
        //Triangles dizisini olu�tururken kullanaca��m�z index de�i�keni
        int t = 0;

        //��i�e for d�ng�s� sayesinde yatay ve dikeyde
        //vertexlerimizi olu�turuyoruz.
        for (int x = 0; x <= kareSayisi; x++)
        {
            for (int z = 0; z <= kareSayisi; z++)
            {
                /*
                X ve Z de�erleri ile kare boyutunu �arparak
                olu�turaca��m�z karenin merkezini buluyoruz.
                Ard�ndan da daha �nce belirledi�imiz aral��� ��kararak
                yarataca��m�z vertex pozisyonunu tam olarak belirlemi� oluyoruz.
                Ayr�ca yukseklik dizisine �nceden atad���m�z de�erleri de Y eksenine veriyoruz.
                Ard�ndan vertices dizimize bu pozisyon bilgisini kaydediyoruz.
                */
                vertices[v] = new Vector3((x * kareBoyutu) - vertexAralik, yukseklik[x, z], (z * kareBoyutu) - vertexAralik);
                /*
                Her d�ng� sonras�nda v index de�i�kenimizi artt�rarak
                bir sonraki vertex de�erine ge�ip bunu kaydediyoruz.
                ��i�e d�ng� kullan�p tek boyutlu diziye veri kaydederken
                bu y�ntem basit ve kullan��l�. Farkl� y�ntemler de deneyebilirsiniz.
                */
                v++;
            }
        }

        v = 0;
        //Karelerin olu�turulma s�ras� - Mesh G�r�n�m�
        /* 
        3 7 11 15
        2 6 10 14
        1 5 9  13
        0 4 8  12
        */
        //Karelerin dizi i�erisinde g�r�n�m�
        //0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15

        //Bu k�s�mda kare say�s�n� i�i�e for d�ng�s� ile
        //iki defa d�n�yoruz.
        //Bu sayede kare say�s� 5 girildiyse 
        //5x5 �eklinde kare bir mesh olu�turabiliyoruz.
        for (int x = 0; x < kareSayisi; x++)
        {
            for (int z = 0; z < kareSayisi; z++)
            {
                /*
                Burada yapt���m�z �ey basit�e her bir kareyi dola�mak
                ve karenin sahip oldu�u 6 vertex de�erini kullanarak
                2 adet ��gen olu�turmak.
                Triangles dizisindeki de�erleri ��er ��er alarak
                ��genleri olu�turdu�umuzu unutmay�n.
                */
                triangles[t + 0] = v;
                triangles[t + 1] = v + 1;
                triangles[t + 2] = v + (kareSayisi + 1);
                triangles[t + 3] = v + (kareSayisi + 1);
                triangles[t + 4] = v + 1;
                triangles[t + 5] = v + (kareSayisi + 1) + 1;
                //Vertex de�erimizi artt�rarak bir sonraki vertex'e ge�iyoruz.
                v++;
                /*
                Hali haz�rda 6 tane vertex verisini vererek
                2 adet ��geni olu�turdu�umuz i�in t index de�erimizi
                6 artt�r�p, di�er kareye ge�iyoruz.
                */
                t += 6;
            }
            /*
            Daha �nceki d�ng�den farkl� olarak burada da vertex index
            de�erini artt�rmam�z�n sebebi s�n�ra geliyor olmam�z.
            Bahsetti�im gibi, 5 kare i�in 6 tane nokta oluyor yatay ve dikeyde.
            Ancak en �stteki ve sa�daki vertexlerin devam�nda kareler olu�turmuyoruz.
            Bu y�zden bu de�eri artt�rarak o vertexleri atlay�p i�leme devam ediyoruz.
            */
            v++;
        }
    }

    public void YukseklikVerisi()
    {
        //Vertex say�s�n�n neden karesayisindan 1 fazla oldu�unu PlaneVerisiOlustur
        //fonksiyonu i�erisinde bahsetmi�tim.
        yukseklik = new float[kareSayisi + 1, kareSayisi + 1];
        //Vertex aralik konusunda da ayn� �ekilde PlaneVerisiOlustur i�erisinde bulabilirsiniz.
        float vertexAralik = kareBoyutu * .5f;

        //X ve Z eksenleri �zerinde ilerliyoruz.
        //2 boyutlu bir grid olu�turmu�tuk sonu�ta asl�nda.
        for (int x = 0; x <= kareSayisi; x++)
        {
            for (int z = 0; z <= kareSayisi; z++)
            {
                /*
                x ve z de�erlerini kare boyutu ile �arp�p aral�k de�erini ��kararak
                vertex'in olaca�� konumu belirliyoruz.
                Bu de�erleri asl�nda PlaneVerisiOlustur i�erisinde de tekrar hesapl�yoruz.
                �nce onu �a��r�p, konumlar� hesaplat�p sonra y�kseklik verisi hesaplad���m�z 
                bir yap�y� kurmay� size �dev olarak b�rak�yorum. 
                Bu haliyle ayn� i�i 2 kere yap�yoruz sonu�ta, daha optimize olmal� :)
                Ayr�ca yaln�zca X ekseninde hareket etmesini istedi�im i�in, ge�en zaman� da ilk de�i�kene ekliyorum.
                */
                float xVerisi = (x * kareBoyutu) - vertexAralik + Time.time;
                float zVerisi = (z * kareBoyutu) - vertexAralik;
                /*Y�ksekli�ini belirleyece�imiz vertex'in ger�ek konum de�erlerini 
                PerlinNoise i�in de�i�ken olarak veriyoruz.
                Bu fonksiyon bize 0 ve 1 aras� bir de�er verecek. maksYukseklik de�eriyle de �arparak bunu
                yukseklik adl� dizimize kaydediyoruz.
                */
                float vertexYuksekligi = Mathf.PerlinNoise(xVerisi * perlinScale, zVerisi * perlinScale);
                yukseklik[x, z] = vertexYuksekligi * maksYukseklik;
            }
        }
    }

    /*
    Burada yapt���m�z �ey s�ras�yla ��yle
    Mesh verisini temizliyoruz.
    Mesh'i olu�turacak vertexlerin atamas�n� yap�yoruz.
    Mesh'i olu�turacak ��genlerin atamas�n� yap�yoruz.
    ��genlerin Normal de�erlerini hesaplat�yoruz.
    Ve son olarak Collider bile�enimize bu olu�turdu�umuz 
    mesh verisini ekliyoruz.
    */
    public void MeshOlustur()
    {
        if (mesh != null)
            mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
    }

}