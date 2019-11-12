using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class cheepCheckoutScript : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] MainButtons;
    public KMSelectable[] OtherButtons; //disp, left, right, clear, submit
    public TextMesh[] MainTexts;
    public TextMesh[] OtherTexts;
    public Color[] Colors; //white, red, yellow, green

    //KMAudio.KMAudioRef soundEffect;
    public List<string> birdNames = new List<string> { "Auklet", "Bluebird", "Chickadee", "Dove", "Egret", "Finch", "Godwit", "Hummingbird", "Ibis", "Jay", "Kinglet", "Loon", "Magpie", "Nuthatch", "Oriole", "Pipit", "Quail", "Raven", "Shrike", "Thrush", "Umbrellabird", "Vireo", "Warbler", "Xantus’s Hummingbird", "Yellowlegs", "Zigzag Heron", "[Unicorn Bastard]" };
    public List<double> birdPrices = new List<double> { 3.59, 6.33, 1.99, 2.50, 9.01, 6.90, 5.27, 9.12, 4.10, 8.93, 7.28, 1.23, 3.77, 0.99, 3.14, 1.41, 9.04, 8.00, 4.20, 2.60, 0.01, 9.67, 3.69, 5.51, 2.01, 7.53, 0.00 };
    public List<int> birdPitches = new List<int> { 1, 1, 0, 2, 0, 1, 1, 1, 2, 1, 0, 2, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 0, 2, 0, 0, 0, 2, 0, 0, 2, 1, 1, 2, 1, 2, 1, 2, 0, 0, 2, 1, 0, 1, 0, 2, 2, 1, 2, 2, 2, 1, 1, 2, 1, 0, 2, 2, 1, 2, 0, 2, 1, 0, 0, 1, 2, 0, 0, 1, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 };
    public List<int> numberList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 };
    public List<string> mainNumbers = new List<string> { ".01", ".05", ".10", ".25", " 1 ", " 5 ", "10", "25" };
    public List<string> pitchNames = new List<string> { "Low", "Medium", "High" };
    int selectedBird = 0;
    int pressedButton = 0;
    int unicornPressCount = 0;
    double birdPrice = 0;
    double currentPrice = 0;
    double customerPrice = 0;
    double answerPrice = 0;
    bool validPrice = true;
    bool entering = false;
    bool hasUnicornBird = false;
    bool unicorn = false;

    //soundEffect = Audio.PlaySoundAtTransformWithRef("scream", transform);

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable mButton in MainButtons)
        {
            KMSelectable pressedMButton = mButton;
            mButton.OnInteract += delegate () { mButtonPress(pressedMButton); return false; };
        }

        foreach (KMSelectable oButton in OtherButtons)
        {
            KMSelectable pressedOButton = oButton;
            oButton.OnInteract += delegate () { oButtonPress(pressedOButton); return false; };
        }
    }

    // Use this for initialization
    void Start()
    {
        numberList.Shuffle();
        birdPrice = birdPrices[numberList[0]] + birdPrices[numberList[1]] + birdPrices[numberList[2]] + birdPrices[numberList[3]] + birdPrices[numberList[4]];
        Debug.Log(birdNames[numberList[0]] + " + " + birdNames[numberList[1]] + " + " + birdNames[numberList[2]] + " + " + birdNames[numberList[3]] + " + " + birdNames[numberList[4]] + " = " + birdPrice);

        for (int k = 0; k <= 5; k++)
        {
            Debug.LogFormat("[Cheep Checkout #{0}] Bird {1}: {2}, {3}, {4} ({5}, costs ${6})", moduleId, k + 1, pitchNames[birdPitches[numberList[k] * 3]], pitchNames[birdPitches[numberList[k] * 3 + 1]], pitchNames[birdPitches[numberList[k] * 3 + 2]], birdNames[numberList[k]], birdPrices[numberList[k]]);
        }

        if (numberList[0] == 26 || numberList[1] == 26 || numberList[2] == 26 || numberList[3] == 26 || numberList[4] == 26)
        {
            hasUnicornBird = true;
            Debug.Log("UNICORN BIRD DETECTED");
        }

        customerPrice = UnityEngine.Random.Range(5, 20);
        OtherTexts[0].text = "$" + customerPrice + ".00";

        answerPrice = Math.Round(customerPrice - birdPrice, 2);


        if (Bomb.IsIndicatorOn(Indicator.BOB) && hasUnicornBird)
        {
            unicorn = true;
            Debug.Log("UNICORN IN EFFECT");
            Debug.LogFormat("[Cheep Checkout #{0}] Unicorn rule applies, please repeatedly slap the customer.", moduleId);
        }
        else
        {
            Debug.LogFormat("[Cheep Checkout #{0}] All amounts total to ${1}.", moduleId, birdPrice);
            Debug.LogFormat("[Cheep Checkout #{0}] Customer paid ${1}.", moduleId, customerPrice);
            if (answerPrice <= 0)
            {
                validPrice = false;
                Debug.LogFormat("[Cheep Checkout #{0}] Customer has not paid enough money, submitting nothing required.", moduleId);
            }
            else
            {
                Debug.LogFormat("[Cheep Checkout #{0}] Correct change is ${1}.", moduleId, answerPrice);
            }
        }
    }

    void mButtonPress(KMSelectable pressedMButton)
    {
        pressedMButton.AddInteractionPunch();
        if (moduleSolved == false)
        {
            entering = true;
            for (int j = 0; j < 8; j++)
            {
                if (pressedMButton == MainButtons[j])
                {
                    pressedButton = j;
                }
            }
            if (MainTexts[pressedButton].text == ".01")
            {
                currentPrice += 0.01;
                Audio.PlaySoundAtTransform("dove", transform);
            }
            else if (MainTexts[pressedButton].text == ".05")
            {
                currentPrice += 0.05;
                Audio.PlaySoundAtTransform("dove", transform);
            }
            else if (MainTexts[pressedButton].text == ".10")
            {
                currentPrice += 0.10;
                Audio.PlaySoundAtTransform("dove", transform);
            }
            else if (MainTexts[pressedButton].text == ".25")
            {
                currentPrice += 0.25;
                Audio.PlaySoundAtTransform("dove", transform);
            }
            else if (MainTexts[pressedButton].text == " 1 ")
            {
                currentPrice += 1;
                Audio.PlaySoundAtTransform("falcon", transform);
            }
            else if (MainTexts[pressedButton].text == " 5 ")
            {
                currentPrice += 5;
                Audio.PlaySoundAtTransform("falcon", transform);
            }
            else if (MainTexts[pressedButton].text == "10")
            {
                currentPrice += 10;
                Audio.PlaySoundAtTransform("falcon", transform);
            }
            else if (MainTexts[pressedButton].text == "25")
            {
                currentPrice += 25;
                Audio.PlaySoundAtTransform("falcon", transform);
            }

            OtherTexts[0].color = Colors[2];
            Debug.Log(currentPrice);

            if (currentPrice % 1 == 0)
            {
                OtherTexts[0].text = "$" + currentPrice + ".00";
            }
            else
            {
                OtherTexts[0].text = "$" + currentPrice.ToString().PadRight((Math.Floor(currentPrice).ToString() + ".").Length + 2, '0');
            }

            mainNumbers.Shuffle();
            MainTexts[0].text = mainNumbers[0];
            MainTexts[1].text = mainNumbers[1];
            MainTexts[2].text = mainNumbers[2];
            MainTexts[3].text = mainNumbers[3];
            MainTexts[4].text = mainNumbers[4];
            MainTexts[5].text = mainNumbers[5];
            MainTexts[6].text = mainNumbers[6];
            MainTexts[7].text = mainNumbers[7];
        }
    }

    void oButtonPress(KMSelectable pressedOButton)
    {
        pressedOButton.AddInteractionPunch();
        if (moduleSolved == false)
        {
            if (pressedOButton == OtherButtons[0]) // DISPLAY
            {
                StartCoroutine(PlaySounds());
            }
            else if (pressedOButton == OtherButtons[1]) //LEFT
            {
                if (selectedBird != 0)
                {
                    selectedBird -= 1;
                }
                else
                {
                    Audio.PlaySoundAtTransform("african-grey-parrot", transform);
                }
                Debug.Log("Selected Bird: " + selectedBird);
            }
            else if (pressedOButton == OtherButtons[2]) //RIGHT
            {
                if (selectedBird != 4)
                {
                    selectedBird += 1;
                }
                else
                {
                    Audio.PlaySoundAtTransform("african-grey-parrot", transform);
                }
                Debug.Log("Selected Bird: " + selectedBird);
            }
            else if (pressedOButton == OtherButtons[3]) //CLEAR
            {
                OtherTexts[0].color = Colors[0];
                currentPrice = 0;
                entering = false;
                OtherTexts[0].text = "$" + customerPrice + ".00";
                Audio.PlaySoundAtTransform("crow", transform);
            }
            else if (pressedOButton == OtherButtons[4]) //SUBMIT
            {
                if (unicorn == false)
                {
                    if (entering == false)
                    {
                        if (validPrice == true)
                        {
                            Debug.LogFormat("[Cheep Checkout #{0}] Customer slapped, but they have paid enough money, now they're pissed, here's a strike.", moduleId);
                            GetComponent<KMBombModule>().HandleStrike();
                            Audio.PlaySoundAtTransform("crow", transform);
                            StartCoroutine(RedText());
                        }
                        else
                        {
                            Debug.LogFormat("[Cheep Checkout #{0}] Customer slapped successfully.", moduleId);
                            Audio.PlaySoundAtTransform("crow", transform);
                            StartCoroutine(OneSecond());
                        }
                    }
                    else
                    {
                        Debug.Log(currentPrice + " | " + answerPrice);
                        if (currentPrice.ToString("N") == answerPrice.ToString("N"))
                        {
                            Debug.LogFormat("[Cheep Checkout #{0}] Correct amount of change given, module solved.", moduleId);
                            moduleSolved = true;
                            StartCoroutine(CorrectAnswer());
                        }
                        else
                        {
                            Debug.LogFormat("[Cheep Checkout #{0}] Incorrect amount of change given ({1}), module striked.", moduleId, currentPrice.ToString("N"));
                            Audio.PlaySoundAtTransform("crow", transform);
                            GetComponent<KMBombModule>().HandleStrike();
                            StartCoroutine(RedText());
                        }
                    }
                } else
                {
                    unicornPressCount += 1;
                    Debug.LogFormat("[Cheep Checkout #{0}] Ow!", moduleId);
                    if (unicornPressCount >= 15)
                    {
                        moduleSolved = true;
                        StartCoroutine(UnicornSolve());
                    } else
                    {
                        Audio.PlaySoundAtTransform("crow", transform);
                    }
                }
            }
        }
    }

    IEnumerator PlaySounds()
    {
        Debug.Log(birdPitches[numberList[selectedBird] * 3]);
        if (birdPitches[numberList[selectedBird] * 3] == 0)
        {
            Audio.PlaySoundAtTransform("low1", transform);
        }
        else if (birdPitches[numberList[selectedBird] * 3] == 1)
        {
            Audio.PlaySoundAtTransform("med1", transform);
        }
        else if (birdPitches[numberList[selectedBird] * 3] == 2)
        {
            Audio.PlaySoundAtTransform("high1", transform);
        }

        yield return new WaitForSeconds(0.68f);

        Debug.Log(birdPitches[numberList[selectedBird] * 3 + 1]);
        if (birdPitches[numberList[selectedBird] * 3 + 1] == 0)
        {
            Audio.PlaySoundAtTransform("low2", transform);
        }
        else if (birdPitches[numberList[selectedBird] * 3 + 1] == 1)
        {
            Audio.PlaySoundAtTransform("med2", transform);
        }
        else if (birdPitches[numberList[selectedBird] * 3 + 1] == 2)
        {
            Audio.PlaySoundAtTransform("high2", transform);
        }

        yield return new WaitForSeconds(0.68f);

        Debug.Log(birdPitches[numberList[selectedBird] * 3 + 2]);
        if (birdPitches[numberList[selectedBird] * 3 + 2] == 0)
        {
            Audio.PlaySoundAtTransform("low3", transform);
        }
        else if (birdPitches[numberList[selectedBird] * 3 + 2] == 1)
        {
            Audio.PlaySoundAtTransform("med3", transform);
        }
        else if (birdPitches[numberList[selectedBird] * 3 + 2] == 2)
        {
            Audio.PlaySoundAtTransform("high3", transform);
        }
    }

    IEnumerator OneSecond()
    {
        Audio.PlaySoundAtTransform("robin", transform);
        for (int i = 0; i < 2; i++)
        {
            OtherTexts[0].text = "ONE SECOND";
            yield return new WaitForSeconds(0.375f);
            OtherTexts[0].text = "ONE SECOND.";
            yield return new WaitForSeconds(0.375f);
            OtherTexts[0].text = "ONE SECOND..";
            yield return new WaitForSeconds(0.375f);
            OtherTexts[0].text = "ONE SECOND...";
            yield return new WaitForSeconds(0.375f);
        }
        customerPrice += UnityEngine.Random.Range(5,20);
        OtherTexts[0].text = "$" + customerPrice + ".00";
        Debug.LogFormat("[Cheep Checkout #{0}] New price is ${1}.", moduleId, customerPrice);
        answerPrice = customerPrice - birdPrice;
        Debug.Log(answerPrice);
        if (answerPrice <= 0)
        {
            validPrice = false;
            Debug.LogFormat("[Cheep Checkout #{0}] Customer has still not paid enough money, submitting nothing required... again.", moduleId);
        } else
        {
            validPrice = true;
        }
    }

    IEnumerator RedText ()
    {
        OtherTexts[0].color = Colors[1];
        yield return new WaitForSeconds(1);
        OtherTexts[0].color = Colors[0];
        currentPrice = 0;
        entering = false;
        OtherTexts[0].text = "$" + customerPrice + ".00";
    }

    IEnumerator CorrectAnswer ()
    {
        OtherTexts[0].color = Colors[3];
        Audio.PlaySoundAtTransform("Rooster", transform);
        yield return new WaitForSeconds(1.645f);
        GetComponent<KMBombModule>().HandlePass();
        MainTexts[0].text = "W";
        MainTexts[1].text = "E";
        MainTexts[2].text = "L";
        MainTexts[3].text = "L";
        MainTexts[4].text = "D";
        MainTexts[5].text = "O";
        MainTexts[6].text = "N";
        MainTexts[7].text = "E";
    }

    IEnumerator UnicornSolve ()
    {
        Debug.LogFormat("[Cheep Checkout #{0}] The customer is now dead, module solved.", moduleId);
        OtherTexts[0].color = Colors[3];
        Audio.PlaySoundAtTransform("Rooster", transform);
        yield return new WaitForSeconds(1.645f);
        GetComponent<KMBombModule>().HandlePass();
        MainTexts[0].text = "U";
        MainTexts[1].text = "N";
        MainTexts[2].text = "I";
        MainTexts[3].text = "C";
        MainTexts[4].text = "O";
        MainTexts[5].text = "R";
        MainTexts[6].text = "N";
        MainTexts[7].text = "<3";
    }
}
