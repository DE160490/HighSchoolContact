const SchoolYears = [];
const Grades = [];
const Subjects = [];
const Classes = [];
const SubjectTeachers = [];
const PersonInformations = [];
async function fetchDataGetSchoolYears() {
    const response = await fetch('/Admin/GetSchoolYears');
    const data = await response.json();
    for (const object of data) {
        var schoolYearId = "";
        var dateStart = "";
        var dateEnd = "";
        for (const key in object) {
            if (Object.hasOwnProperty.call(object, key)) {
                if (key === "schoolYearId") {
                    schoolYearId = object[key];
                } else if (key === "dateStart") {
                    dateStart = object[key];
                } else if (key === "dateEnd") {
                    dateEnd = object[key];
                }
            }
        }
        //console.log(dateStart + " " + dateEnd);
        SchoolYears.push({ SchoolYearID: schoolYearId, DateStart: dateStart, DateEnd: dateEnd, })
    }
}

async function fetchDataGetGrades() {
    const response = await fetch('/Admin/GetGrades');
    const data = await response.json();
    for (const object of data) {
        var gradeId = "";
        var gradeName = "";
        var maxClass = 0;
        var schoolYearId = "";
        for (const key in object) {
            if (Object.hasOwnProperty.call(object, key)) {
                if (key === "gradeId") {
                    gradeId = object[key];
                } else if (key === "gradeName") {
                    gradeName = object[key];
                } else if (key === "maxClass") {
                    maxClass = object[key];
                } else if (key === "schoolYearId") {
                    schoolYearId = object[key];
                }
            }
        }
        Grades.push({ GradeID: gradeId, GradeName: gradeName, MaxClass: maxClass, SchoolYearID: schoolYearId })
    }
}

async function fetchDataGetClasses() {
    const response = await fetch('/Admin/GetClasses');
    const data = await response.json();
    for (const object of data) {
        var classId = "";
        var className = "";
        var numberOfStudent = 0;
        var gradeId = "";

        for (const key in object) {
            if (Object.hasOwnProperty.call(object, key)) {
                if (key === "classId") {
                    classId = object[key];
                } else if (key === "className") {
                    className = object[key];
                } else if (key === "numberOfStudent") {
                    numberOfStudent = object[key];
                } else if (key === "gradeId") {
                    gradeId = object[key];
                }
            }
        }
        Classes.push({ ClassID: classId, ClassName: className, NumberOfStudent: numberOfStudent, GradeID: gradeId })
    }
}

async function fetchDataGetSubjectTeacher() {
    const response = await fetch('/Admin/GetSubjectTeacher');
    const data = await response.json();
    for (const object of data) {
        var teacherId = "";
        var classId = "";
        var subjectId = "";

        for (const key in object) {
            if (Object.hasOwnProperty.call(object, key)) {
                if (key === "teacherId") {
                    teacherId = object[key];
                } else if (key === "classId") {
                    classId = object[key];
                } else if (key === "subjectId") {
                    subjectId = object[key];
                }
            }
        }
        SubjectTeachers.push({ TeacherID: teacherId, ClassID: classId, SubjectID: subjectId })
    }
}

async function fetchDataGetSubjects() {
    const response = await fetch('/Admin/GetSubjects');
    const data = await response.json();
    for (const object of data) {
        var subjectId = "";
        var subjectName = "";
        var description = "";

        for (const key in object) {
            if (Object.hasOwnProperty.call(object, key)) {
                if (key === "subjectId") {
                    subjectId = object[key];
                } else if (key === "subjectName") {
                    subjectName = object[key];
                } else if (key === "description") {
                    description = object[key];
                }
            }
        }
        Subjects.push({ SubjectID: subjectId, SubjectName: subjectName, Description: description })
    }
}

async function fetchDataGetPersonInformation() {
    const response = await fetch('/Admin/GetPersonInformation');
    const data = await response.json();
    for (const object of data) {
        var id = "";
        var fullname = "";

        for (const key in object) {
            if (Object.hasOwnProperty.call(object, key)) {
                if (key === "id") {
                    id = object[key];
                } else if (key === "fullname") {
                    fullname = object[key];
                } else if (key === "description") {
                    description = object[key];
                }
            }
        }
        PersonInformations.push({ ID: id, Fullname: fullname })
    }
}

const main = async () => {
    await fetchDataGetSchoolYears();
    await fetchDataGetGrades();
    await fetchDataGetClasses();
    await updateWeek();

    await fetchDataGetSubjectTeacher();
    await fetchDataGetSubjects();
    await fetchDataGetPersonInformation();
    await updateTeacher();

    //await console.log(Grades);
    //await console.log(Classes);
    /*await console.log(SubjectTeachers);*/
    //await console.log(Subjects);
    //await console.log(PersonInformations);
};


main();
async function updateTeacher() {
    const selectteacher = document.querySelector('select[name="teacherID"]');
    for (const person of PersonInformations) {
        let count = 0;
        for (const teacher of SubjectTeachers) {     
            if (person.ID === teacher.TeacherID && count === 0) {
                count++;
                const option = document.createElement('option');
                option.value = teacher.TeacherID;
                option.textContent = teacher.TeacherID + " - " + person.Fullname;
                selectteacher.appendChild(option);
            }
        }
    }
}
async function updateWeek() {

    var datenow = new Date();

    const selectweekBegin = document.querySelector("select[name='datetime']");

    const options = selectweekBegin.querySelectorAll('option');
    options.forEach((option) => option.remove());

    for (let i = 0; i < 10; i++) {
        if (datenow.getDay() !== 1) {
            datenow = new Date(datenow.getTime() - (24 * 60 * 60 * 1000));
        }
    }

    for (const schoolYear of SchoolYears) {
        var start = new Date(schoolYear.DateStart.split('T')[0]);
        var end = new Date(schoolYear.DateEnd.split('T')[0]);

        if (start <= datenow && end >= datenow) {

            console.log(schoolYear.DateStart + "  -  " + schoolYear.DateEnd);
            
            console.log("Convert start: " + start);
            console.log("Convert end: " + end);

            var ok = true;

            if (end.getDay() !== 0) {
                ok = false;
            }

            for (let i = 0; i < 10; i++) {
                if (start.getDay() !== 1) {
                    start = new Date(start.getTime() - (24 * 60 * 60 * 1000));
                }

                if (end.getDay() !== 0) {
                    end = new Date(end.getTime() + (24 * 60 * 60 * 1000));
                }
            }

            let weeks = Math.ceil((end - start) / (7 * 24 * 60 * 60 * 1000));

            if (!ok) {
                weeks = weeks + 1;
            }


            for (let i = 0; i < weeks; i++) {

                var weekStart = new Date(start.getTime() + (i * 7 * 24 * 60 * 60 * 1000));
                var weekEnd = new Date(weekStart.getTime() + (6 * 24 * 60 * 60 * 1000));

                const option = document.createElement('option');
                option.value = await convertTimeFormat2(weekStart) + "&&" + await convertTimeFormat2(weekEnd);
                option.textContent = await convertTimeFormat1(weekStart) + " To " + await convertTimeFormat1(weekEnd);

                if (datenow.getFullYear() === weekStart.getFullYear() && datenow.getMonth() === weekStart.getMonth() && datenow.getDate() === weekStart.getDate()) {
                    option.setAttribute("selected", true);
                }
                console.log(datenow + " ==== " + weekStart);
                selectweekBegin.appendChild(option);

            }
        }
    }
}

async function convertTimeFormat1(time) {
    var convert = "";
    if (time.getDate() <= 9) {
        if (time.getMonth() < 9) {
            convert = "0" + time.getDate() + "/" + "0" + (time.getMonth() + 1);
        } else {
            convert = "0" + time.getDate() + "/" + (time.getMonth() + 1);
        }

    } else {
        if (time.getMonth() < 9) {
            convert = time.getDate() + "/" + "0" + (time.getMonth() + 1);
        } else {
            convert = time.getDate() + "/" + (time.getMonth() + 1);
        }
    }
    return convert;
}

async function convertTimeFormat2(time) {
    var convert = "";
    if (time.getDate() <= 9) {
        if (time.getMonth() < 9) {
            convert = "0" + (time.getMonth() + 1) + "/" + "0" + time.getDate() + "/" + time.getFullYear();
        } else {
            convert = (time.getMonth() + 1) + "/" + "0" + time.getDate() + "/" + time.getFullYear();
        }

    } else {
        if (time.getMonth() < 9) {
            convert = "0" + (time.getMonth() + 1) + "/" + time.getDate() + "/" + time.getFullYear();
        } else {
            convert = (time.getMonth() + 1) + "/" + time.getDate() + "/" + time.getFullYear();
        }
    }
    return convert;
}
