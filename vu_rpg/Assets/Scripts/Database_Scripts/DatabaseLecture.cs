using System;
using System.Collections.Generic;


public partial class Database {

    static void Initialize_Lecture() {
        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS Lecture (
                            lecture_id INTEGER NOT NULL PRIMARY KEY,
                            lecture_title TEXT NOT NULL,
                            lecture_url TEXT,
                            lecture_owner TEXT,
                            fk_subject_name TEXT)");

        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS LectureAttend (
                            attend_id INTEGER NOT NULL PRIMARY KEY,
                            attend_date DATETIME DEFAULT CURRENT_TIMESTAMP,
                            attend_value INTEGER DEFAULT 0,
                            has_attended INTEGER DEFAULT 0,
                            watch_time DATETIME,
                            fk_account TEXT NOT NULL,
                            fk_lecture_id INTEGER NOT NULL)");

        ExecuteNoReturn(@"CREATE TABLE IF NOT EXISTS LectureBreakPoint (
                            break_id INTEGER NOT NULL PRIMARY KEY,
                            break_time DATETIME,
                            fk_lecture_id INTEGER NOT NULL)");
    }

}