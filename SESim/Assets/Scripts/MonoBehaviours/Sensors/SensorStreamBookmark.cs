
public class SensorStreamBookmark {

    public string BookmarkName;
    public System.DateTime StartDateTime, EndDateTime;

    public string GetSensorStreamBookmarkString(bool useStartDateTime)
    {
        if (useStartDateTime)
            return GetSensorStreamStartString();
        return GetSensorStreamEndString();
    }

    public string GetSensorStreamStartString()
    {
        return "BookmarkName" + "-" + BookmarkName + "-"
            + "StartDate" + "-" + StartDateTime.ToShortDateString() + "-" 
            + "StartTime" + "-" + StartDateTime.ToShortTimeString();
    }

    public string GetSensorStreamEndString()
    {
        return "BookmarkName" + "-" + BookmarkName + "-"
            + "EndDate" + "-" + EndDateTime.ToShortDateString() + "-"
            + "EndTime" + "-" + EndDateTime.ToShortTimeString();
    }

    public bool DoesSensorReadingOccurInBookmark(BaseSensorReading aSensorReading)
    {
        if ((StartDateTime <= aSensorReading.dateTime) && (aSensorReading.dateTime <= EndDateTime))
            return true;
        return false;
    }
}
