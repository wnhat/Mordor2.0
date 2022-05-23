namespace CoreClass.DICSEnum
{
    /// <summary>
    /// 对应cell log 文件中的 header，当设备log储存项目发生改变时请修改该项目；
    /// </summary>
    enum FileHeaderEnum
    {
        DATE,
        MODEL,
        OPERATIONID,
        PRODUCTTYPE,
        LINEMODE,
        INNERID,
        VCRID,
        STAGEID,
        MTPJUDGE,
        MTPRESULT,
        MTPTACTTIME,
        JNCD,
        MTP_CODE,
        AVITACTTIME,
        AVIGRABTACTTIME,
        AVICLASSTACTTIME,
        SVITACTTIME,
        SVIGRABTACTTIME,
        SVICLASSTACTTIME,
        MERGE1JUDGE,
        APPTACTTIME,
        APPGRABTACTTIME,
        APPCLASSTACTTIME,
        MERGETOOLJUDGE,
        MERGETOOLCODE,
        DEFECTCODETYPE,
        DEFECTNAME,
        MVIJUDGE,
        MVIRESULT,
        MVITACTTIME,
        MVIUSER,
        MVIUSERID,
        TSPJUDGE,
        TSPTACTTIME,
        LASTRESULT,
        LASTRESULTNAME,
        LASTJUDGE,
        DEFECTCODE,
        MERGE1TACTTIME,
        MERGE2TACTTIME,
        AVIJUDGE,
        AVICODE,
        SVIJUDGE,
        SVICODE,
        APPJUDGE,
        APPCODE
    }
    /// <summary>
    /// 对应检查pc 文件中的 Index，当设备log储存项目发生改变时请修改该项目；
    /// </summary>
    enum ResultIndexHeader
    {
        StartTimeDay = 2,
        StartTimeHour = 3,
        EndTimeDay = 4,
        EndTimeHour = 5,
        Type = 6,
    }
    enum ResultIndexPanelData
    {
        LotID = 3,
        PanelID = 4,
        Judge = 5,
        RecipeName = 8,
        ROI_START_X = 9,
        ROI_START_Y = 10,
        ROI_END_X = 11,
        ROI_END_Y = 12,
    }
    enum ResultIndexDefectData
    {
        DEFECT_NO = 2,
        IMG_NAME,
        DEF_TYPE,
        DEFECT_CODE,
        PIXEL_START_X,
        PIXEL_START_Y,
        PIXEL_END_X,
        PIXEL_END_Y,
        GATE_START_NO,
        DATA_START_NO ,
        GATE_END_NO,
        DATA_END_NO ,
        COORD_START_X ,
        COORD_START_Y ,
        COORD_END_X ,
        COORD_END_Y,
        DEF_SIZE   ,
        DEF_IMG_FILE  ,
        DRAW_RECT  ,
        IMG_SIZE_X ,
        IMG_SIZE_Y ,
        CAM_NO  ,
        PS_FLAG ,
    }
}