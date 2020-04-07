using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The raw sensor stream class. This object is used to store and manage all
/// sensor readings for a unique session.
/// </summary>
public class SensorStream
{
    private int sessionID;
    private List<BaseSensorReading> sensorReadings;
    private List<SensorStreamBookmark> sensorBookmarks;

    public int SessionID
    {
        get
        {
            return sessionID;
        }

        set
        {
            sessionID = value;
        }
    }

    public SensorStream(int sessionID)
    {
        SessionID = sessionID;
        sensorReadings = new List<BaseSensorReading>();
        sensorBookmarks = new List<SensorStreamBookmark>();
    }

    public void Clear()
    {
        sensorReadings.Clear();
        sensorBookmarks.Clear();
    }

    public void AddSensorData(BaseSensorReading newSensorReading)
    {
        sensorReadings.Add(newSensorReading);
    }

    public void AddSensorBookmark(string bookmarkName, System.DateTime startDateTime)
    {
        if (!DoesBookmarkExist(bookmarkName))
        {
            SensorStreamBookmark newBookmark = new SensorStreamBookmark
            {
                BookmarkName = bookmarkName,
                StartDateTime = startDateTime
            };

            sensorBookmarks.Add(newBookmark);
        }
    }

    public bool DoesBookmarkExist(string bookmarkName)
    {
        foreach (SensorStreamBookmark aBookmark in sensorBookmarks)
            if (aBookmark.BookmarkName.Equals(bookmarkName))
                return true;
        return false;
    }

    public bool AnnotateEndOfSensorBookmark(string bookmarkName, System.DateTime endDateTime)
    {
        SensorStreamBookmark theBookmark;
        if (TryGetBookmark(bookmarkName, out theBookmark))
        {
            theBookmark.EndDateTime = endDateTime;
            return true;
        }
        else
        {
            Debug.LogError("Sensor bookmark not found.");
        }

        return false;
    }

    private bool TryGetBookmark(string bookmarkName, out SensorStreamBookmark bookmark)
    {
        bookmark = null;
        foreach (SensorStreamBookmark aBookmark in sensorBookmarks)
        {
            if (aBookmark.BookmarkName.Equals(bookmarkName))
            {
                bookmark = aBookmark;
                return true;
            }
        }

        Debug.LogError("Sensor bookmark with name: '" + bookmarkName + "' does not exist in sensor stream.");
        return false;
    }

    public List<BaseSensorReading> GetSensorReadingsWithinSensorBookmark(SensorStreamBookmark aBookmark)
    {
        List<BaseSensorReading> sensorReadingsWithinBookmark = new List<BaseSensorReading>();
        foreach (BaseSensorReading aSensorReading in sensorReadings)
            if (aBookmark.DoesSensorReadingOccurInBookmark(aSensorReading))
                sensorReadingsWithinBookmark.Add(aSensorReading);
        return sensorReadingsWithinBookmark;
    }

    public List<BaseSensorReading> GetSensorReadings()
    {
        return sensorReadings;
    }

    public List<SensorStreamBookmark> GetSensorStreamBookmarks()
    {
        return sensorBookmarks;
    }
}
