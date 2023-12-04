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
    await fetchDataGetSubjectTeacher();
    await fetchDataGetSubjects();
    await fetchDataGetPersonInformation();
    await updateWeek();

    //await console.log(Grades);
    //await console.log(Classes);
    /*await console.log(SubjectTeachers);*/
    //await console.log(Subjects);
    //await console.log(PersonInformations);
};

main();

updateWeek();
async function updateWeek() {
    var week = document.querySelector("p.week").textContent;
    var start = new Date(week.substr(1, 10));
    var end = new Date(week.substr(13, 10));
    for (let i = 0; i < 10; i++) {
        if (start.getDay() !== 1) {
            start = new Date(start.getTime() - (24 * 60 * 60 * 1000));
        }

        if (end.getDay() !== 0) {
            end = new Date(end.getTime() + (24 * 60 * 60 * 1000));
        }
    }

    var weeklast = await convertTimeFormat2(new Date(start.getTime() - (7 * 24 * 60 * 60 * 1000))) + "&&" + await convertTimeFormat2(new Date(end.getTime() - (7 * 24 * 60 * 60 * 1000)));
    var weeknext = await convertTimeFormat2(new Date(start.getTime() + (7 * 24 * 60 * 60 * 1000))) + "&&" + await convertTimeFormat2(new Date(end.getTime() + (7 * 24 * 60 * 60 * 1000)));

    var form1 = document.createElement('form');
    form1.setAttribute('method', 'post');
    form1.setAttribute('action', '/Teacher/TeacherTimeTable');
    var input1 = document.createElement('input');
    input1.setAttribute('name', 'week');
    input1.setAttribute('hidden', 'true');
    input1.setAttribute('value', weeklast);
    var btn1 = document.createElement('button');
    btn1.setAttribute('type', 'submit');
    btn1.textContent = "Tuần trước";
    form1.appendChild(input1);
    form1.appendChild(btn1);

    var form2 = document.createElement('form');
    form2.setAttribute('method', 'post');
    form2.setAttribute('action', '/Teacher/TeacherTimeTable');
    var input2= document.createElement('input');
    input2.setAttribute('name', 'week');
    input2.setAttribute('hidden', 'true');
    input2.setAttribute('value', weeknext);
    var btn2 = document.createElement('button');
    btn2.setAttribute('type', 'submit');
    btn2.textContent = "Tuần sau";
    form2.appendChild(btn2);
    form2.appendChild(input2);
    //var div = document.querySelector("div.time_page");
    //div.appendChild(form1);
    //div.appendChild(form2);
    var div1 = document.querySelector("div.btn_before");
    div1.appendChild(form1);
    var div2 = document.querySelector("div.btn_last");
    div2.appendChild(form2);
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

async function convertTimeFormat3(time) {
    var convert = "";
    if (time.getDate() <= 9) {
        if (time.getMonth() < 9) {
            convert = "0" + time.getDate() + "/" + "0" + (time.getMonth() + 1) + "/" + time.getFullYear();
        } else {
            convert = "0" + time.getDate() + "/" + (time.getMonth() + 1) + "/" + time.getFullYear();
        }

    } else {
        if (time.getMonth() < 9) {
            convert = time.getDate() + "/" + "0" + (time.getMonth() + 1) + "/" + time.getFullYear();
        } else {
            convert = time.getDate() + "/" + (time.getMonth() + 1) + "/" + time.getFullYear();
        }
    }
    return convert;
}
