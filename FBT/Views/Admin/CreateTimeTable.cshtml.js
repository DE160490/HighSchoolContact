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

/////////////////////////////////////// Validate ///////////////////////////////////////

//// Cập nhật School Years khi load trang
async function updateSchoolYears() {
    var datetime = new Date();
    const select = document.getElementById('class');
    for (const schoolYear of SchoolYears) {
        if (Date.parse(schoolYear.DateEnd) >= datetime) {
            for (const grade of Grades) {
                if (schoolYear.SchoolYearID == grade.SchoolYearID) {
                    for (const classs of Classes) {
                        if (classs.GradeID == grade.GradeID) {
                            const option = document.createElement('option');
                            option.value = classs.ClassID;
                            option.textContent = schoolYear.DateStart.split('-')[0] + '-' + schoolYear.DateEnd.split('-')[0] + ' ' + classs.ClassName;
                            select.appendChild(option);
                        }
                    }
                }
            }
        }
    }
}

//// Cập nhật ràng buộc tuần học bắt đầu, kết thúc của thời khóa biểu nếu đã chọn năm học
async function updateWeekbySchoolYear() {

    var classID = document.querySelector('select[name="class"]').value;
    var gradeID = Classes.find(e => e.ClassID === classID);
    var schoolYear = Grades.find(e => e.GradeID === gradeID.GradeID);
    var datetimeSY = SchoolYears.find(e => e.SchoolYearID === schoolYear.SchoolYearID);

    document.querySelector('input[name="weekbegins"]').setAttribute("min", datetimeSY.DateStart.split('T')[0]);
    document.querySelector('input[name="weekbegins"]').setAttribute("max", datetimeSY.DateEnd.split('T')[0]);

    document.querySelector('input[name="weekends"]').setAttribute("min", datetimeSY.DateStart.split('T')[0]);
    document.querySelector('input[name="weekends"]').setAttribute("max", datetimeSY.DateEnd.split('T')[0]);
}

const main = async () => {
    await fetchDataGetSchoolYears();
    await fetchDataGetGrades();
    await fetchDataGetClasses();

    await updateSchoolYears();
    await updateWeekbySchoolYear();

    await fetchDataGetSubjectTeacher();
    await fetchDataGetSubjects();
    await fetchDataGetPersonInformation();

    //await console.log(SchoolYears);
    //await console.log(Grades);
    //await console.log(Classes);
    await console.log(SubjectTeachers);
    //await console.log(Subjects);
    //await console.log(PersonInformations);
};


main();

updateTimebylecture();


//// Cập nhật ràng buộc tuần kết thúc của thời khóa biểu sau khi chọn tuần học bắt đầu
function updateWeekEndbyWeekStart() {
    var timeweek = document.querySelector('input[name="weekbegins"]').value;
    document.querySelector('input[name="weekends"]').setAttribute("min", timeweek);
}

//// Cập nhật ràng buộc thời gian bắt đầu, kết thúc tiết học sau khi đã chọn buổi học
function updateTimebylecture() {
    var lecture = document.querySelector('select[name="lecture"]').value;
    if (lecture === 'Morning') {
        document.querySelector('input[name="timestart"]').setAttribute("min", "00:00");
        document.querySelector('input[name="timestart"]').setAttribute("max", "11:59");
        document.querySelector('input[name="timeend"]').setAttribute("min", "00:00");
        document.querySelector('input[name="timeend"]').setAttribute("max", "11:59");
    } else if (lecture === 'Afternoon') {
        document.querySelector('input[name="timestart"]').setAttribute("min", "12:00");
        document.querySelector('input[name="timestart"]').setAttribute("max", "23:59");
        document.querySelector('input[name="timeend"]').setAttribute("min", "12:00");
        document.querySelector('input[name="timeend"]').setAttribute("max", "23:59");
    }
}

//// Cập nhật ràng buộc thời gian kết thúc biểu học sau khi chọn thời gian bắt đầu
function updateTimeEndByTimeStart() {
    var timeweek = document.querySelector('input[name="timestart"]').value;
    document.querySelector('input[name="timeend"]').setAttribute("min", timeweek);
}


/////////////////////////////////////// Hàm chức năng, xử lý ///////////////////////////////////////

//// Cập nhật Môn học và Giáo viên cho Admin kéo thả tạo thời khóa biểu sau khi chọn tuần học kết thúc
function updateSubjectTeacher() {

    const tableBody = document.getElementById('subject');
    const classID = document.querySelector("select[name='class']").value;
    console.log(classID)
    var id = 0;

    for (const subject of Subjects) {
        for (const teaching of SubjectTeachers.filter(item => item.ClassID === classID)) {
            if (teaching.SubjectID === subject.SubjectID) {
                for (const person of PersonInformations) {
                    if (person.ID === teaching.TeacherID) {

                        const tableRow = document.createElement('tr');

                        const subject_Name = document.createElement('td');
                        const subject_ID = document.createElement('td');
                        const teacher_Name = document.createElement('td');
                        const teacher_ID = document.createElement('td');

                        subject_Name.textContent = subject.SubjectName;
                        subject_ID.textContent = subject.SubjectID;
                        teacher_Name.textContent = person.Fullname;
                        teacher_ID.textContent = person.ID;

                        subject_ID.setAttribute("draggable", "true");
                        teacher_ID.setAttribute("draggable", "true");

                        subject_ID.setAttribute("id", id);
                        subject_ID.setAttribute("onmouseenter", "drag(" + id + ")");
                        id++;

                        teacher_ID.setAttribute("id", id);
                        teacher_ID.setAttribute("onmouseenter", "drag(" + id + ")");
                        id++;

                        tableRow.appendChild(subject_Name);
                        tableRow.appendChild(subject_ID);
                        tableRow.appendChild(document.createElement('td'));
                        tableRow.appendChild(teacher_Name);
                        tableRow.appendChild(teacher_ID);
                        tableBody.append(tableRow);
                    }
                }
            }
        }
    }
}


//// Hàm cho phép kéo thả trong tạo thời khóa biểu
function drag(id) {
    const element_td = document.querySelector('td[id="' + id + '"]');
    const element_input = document.querySelector('input[type="text"]');

    element_td.addEventListener('dragstart', (event) => {
        event.dataTransfer.setData('text/plain', element_td.textContent);
    });

    element_input.addEventListener('dragover', (event) => {
        if (event.dataTransfer.types.includes('text/plain')) {
            event.preventDefault();
        }
    });

    element_input.addEventListener('drop', (event) => {
        event.preventDefault();
        element_input.value = event.dataTransfer.getData('text/plain');
    });
}

//// Hàm tạo trường input của các thứ trong tuần 
function updateDayofWeekSubject() {
    const weekbeginsValue = document.querySelector("input[name='weekbegins']").value;
    const weekendsValue = document.querySelector("input[name='weekends']").value;

    const convertbegin = new Date(Date.parse(weekbeginsValue));
    const convertend = new Date(Date.parse(weekendsValue));

    var CheckList = [];
    var DayList = [];
    var DayNumber = [];

    var checkMon = false;
    var checkTue = false;
    var checkWed = false;
    var checkThu = false;
    var checkFri = false;
    var checkSat = false;
    var checkSun = false;

    for (let i = convertbegin.getTime(); i <= convertend.getTime(); i += 1000 * 60 * 60 * 24) {
        const date = new Date(i);
        const day = date.toDateString().split(' ')[0];
        if (day == 'Mon') {
            checkMon = true;
        } if (day == 'Tue') {
            checkTue = true;
        } else if (day == 'Wed') {
            checkWed = true;
        } else if (day == 'Thu') {
            checkThu = true;
        } else if (day == 'Fri') {
            checkFri = true;
        } else if (day == 'Sat') {
            checkSat = true;
        } else if (day == 'Sun') {
            checkSun = true;
        }
    }

    CheckList.push(checkMon, checkTue, checkWed, checkThu, checkFri, checkSat, checkSun);
    DayList.push("mon", "tue", "wed", "thu", "fri", "sat", "sun");
    DayNumber.push("Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật")
    for (var i = 0; i < 7; i++) {
        const parent = document.querySelector("div[class='field3']");
        if (CheckList[i] == true) {
            const div = document.createElement('div');
            const p = document.createElement('p');
            const inputSub = document.createElement('input');
            const inputTeacher = document.createElement('input');

            div.setAttribute('class', 'field_' + DayList[i] + '');
            p.textContent = DayNumber[i];

            inputSub.setAttribute("type", "text");
            inputSub.setAttribute("name", "subject_" + DayList[i]);
            inputSub.setAttribute("placeholder", "Tên môn học");

            inputTeacher.setAttribute("type", "text");
            inputTeacher.setAttribute("name", "teaching_" + DayList[i]);
            inputTeacher.setAttribute("placeholder", "Giáo viên giảng dạy");


            div.appendChild(p);
            div.appendChild(inputSub);
            div.appendChild(inputTeacher);
            parent.appendChild(div);
        } else {
            const div = document.createElement('div');
            const p = document.createElement('p');
            const inputSub = document.createElement('input');
            const inputTeacher = document.createElement('input');

            div.setAttribute('class', 'field_' + DayList[i] + '');
            p.textContent = DayNumber[i];

            inputSub.setAttribute("type", "text");
            inputSub.setAttribute("name", "subject_" + DayList[i]);
            inputSub.setAttribute("placeholder", "Tên môn học");

            inputTeacher.setAttribute("type", "text");
            inputTeacher.setAttribute("name", "teaching_" + DayList[i]);
            inputTeacher.setAttribute("placeholder", "Giáo viên giảng dạy");

            div.appendChild(p);
            div.appendChild(inputSub);
            div.appendChild(inputTeacher);
            div.setAttribute("hidden", "true");
            parent.appendChild(div);
        }
    }
    //// Gọi hàm tạo Môn học và giáo viên để kéo thả
    updateSubjectTeacher();
}

//updateSchoolYears();
//updateTimebylecture();
//updateWeekbySchoolYear();


////////////////////// Hàm xử lý sau khi Submit form ///////////////////////////////


const form = document.querySelector("form");
form.addEventListener("submit", (event) => {
    var checkOut = false;
    const Schedule = [];
    const TeachingInput = [];
    const classValue = document.getElementById('class').value;
    const lectureValue = document.getElementById('lecture').value;
    const weekbeginsValue = form.querySelector("input[name='weekbegins']").value;
    const weekendsValue = form.querySelector("input[name='weekends']").value;
    const timestartValue = form.querySelector("input[name='timestart']").value;
    const timeendValue = form.querySelector("input[name='timeend']").value;

    if (weekbeginsValue > weekendsValue) {
        console.log("Tuần kết thúc phải lớn hơn tuần bắt đầu");
    }

    if (timestartValue > timeendValue) {
        console.log("Thời gian kết thúc buổi học phải lớn hơn thời gian bắt đầu");
    }

    var start = new Date(Date.parse(weekbeginsValue));
    var end = new Date(Date.parse(weekendsValue));

    // const days = (end - start) / (1000 * 60 * 60 * 24);
    // const weeks = Math.floor(days / 7);
    // console.log(weeks);


    // Lấy thời khóa biểu của thứ 2
    const subjectMon = document.querySelector('input[name="subject_mon"').value;
    const teachingMon = document.querySelector('input[name="teaching_mon"').value;

    if (subjectMon.trim() !== '' && teachingMon.trim() !== '') {
        for (let i = start.getTime(); i <= end.getTime(); i += 1000 * 60 * 60 * 24) {
            const date = new Date(i);
            if (date.getDay() == 1) {
                TeachingInput.push({
                    subject: subjectMon,
                    teacher: teachingMon,
                    day: date.toDateString("yyyy-mm-dd")
                });
            }
        }
        checkOut = true;
    }

    // Lấy thời khóa biểu của thứ 3
    const subjectTue = document.querySelector('input[name="subject_tue"').value;
    const teachingTue = document.querySelector('input[name="teaching_tue"').value;

    if (subjectTue.trim() !== '' && teachingTue.trim() !== '') {
        for (let i = start.getTime(); i <= end.getTime(); i += 1000 * 60 * 60 * 24) {
            const date = new Date(i);
            if (date.getDay() == 2) {
                TeachingInput.push({
                    subject: subjectTue,
                    teacher: teachingTue,
                    day: date.toDateString("yyyy-mm-dd")
                });
            }
        }
        checkOut = true;
    }

    // Lấy thời khóa biểu của thứ 4
    const subjectWed = document.querySelector('input[name="subject_wed"').value;
    const teachingWed = document.querySelector('input[name="teaching_wed"').value;
    if (subjectWed.trim() !== '' && teachingWed.trim() !== '') {
        for (let i = start.getTime(); i <= end.getTime(); i += 1000 * 60 * 60 * 24) {
            const date = new Date(i);
            if (date.getDay() == 3) {
                TeachingInput.push({
                    subject: subjectWed,
                    teacher: teachingWed,
                    day: date.toDateString("yyyy-mm-dd")
                });
            }
        }
        checkOut = true;
    }

    // Lấy thời khóa biểu của thứ 5
    const subjectThu = document.querySelector('input[name="subject_thu"').value;
    const teachingThu = document.querySelector('input[name="teaching_thu"').value;
    if (subjectThu.trim() !== '' && teachingThu.trim() !== '') {
        for (let i = start.getTime(); i <= end.getTime(); i += 1000 * 60 * 60 * 24) {
            const date = new Date(i);
            if (date.getDay() == 4) {
                TeachingInput.push({
                    subject: subjectThu,
                    teacher: teachingThu,
                    day: date.toDateString("yyyy-mm-dd")
                });
            }
        }
        checkOut = true;
    }

    // Lấy thời khóa biểu của thứ 6
    const subjectFri = document.querySelector('input[name="subject_fri"').value;
    const teachingFri = document.querySelector('input[name="teaching_fri"').value;
    if (subjectFri.trim() !== '' && teachingFri.trim() !== '') {
        for (let i = start.getTime(); i <= end.getTime(); i += 1000 * 60 * 60 * 24) {
            const date = new Date(i);
            if (date.getDay() == 5) {
                TeachingInput.push({
                    subject: subjectFri,
                    teacher: teachingFri,
                    day: date.toDateString("yyyy-mm-dd")
                });
            }
        }
        checkOut = true;
    }
    // Lấy thời khóa biểu của thứ 7
    const subjectSat = document.querySelector('input[name="subject_sat"').value;
    const teachingSat = document.querySelector('input[name="teaching_sat"').value;
    if (subjectSat.trim() !== '' && teachingSat.trim() !== '') {
        for (let i = start.getTime(); i <= end.getTime(); i += 1000 * 60 * 60 * 24) {
            const date = new Date(i);
            if (date.getDay() == 6) {
                TeachingInput.push({
                    subject: subjectSat,
                    teacher: teachingSat,
                    day: date.toDateString("yyyy-mm-dd")
                });
            }
        }
        checkOut = true;
    }

    // Lấy thời khóa biểu của chủ nhật
    const subjectSun = document.querySelector('input[name="subject_sun"').value;
    const teachingSun = document.querySelector('input[name="teaching_sun"').value;
    if (subjectSun.trim() !== '' && teachingSun.trim() !== '') {
        for (let i = start.getTime(); i <= end.getTime(); i += 1000 * 60 * 60 * 24) {
            const date = new Date(i);
            if (date.getDay() == 0) {
                TeachingInput.push({
                    subject: subjectSun,
                    teacher: teachingSun,
                    day: date.toDateString("yyyy-mm-dd")
                });
            }
        }
        checkOut = true;
    }

    if (checkOut == false) {
        return false;
    }

    for (const schedule of TeachingInput) {
        const sc = {
            teacherID: schedule.teacher,
            classID: classValue,
            subjectID: schedule.subject,
            weekBegins: weekbeginsValue,
            weekEnds: weekendsValue,
            dayofweek: schedule.day,
            lecture: lectureValue,
            timeStart: timestartValue,
            timeEnd: timeendValue
        }
        Schedule.push(sc);
    }

    for (const c of Schedule) {
        console.log(c);
    }

    pushCData(Schedule);

    event.preventDefault();
});

function pushCData(Schedule) {
    var form = document.createElement("form");
    var input = document.createElement("input");
    form.setAttribute("method", "post");
    form.setAttribute("asp-controller", "Admin");
    form.setAttribute("asp-action", "CreateTimeTable");

    var obj = "";

    for (element of Schedule) {
        obj += "teacherID: " + element.teacherID + ", classID: " + element.classID + ", subjectID: " + element.subjectID + ", weekBegins: " + element.weekBegins + ", weekEnds: " + element.weekEnds + ", dayofweek: " + element.dayofweek + ", lecture: " + element.lecture + ", timeStart: " + element.timeStart + ", timeEnd: " + element.timeEnd + " $@ ";
    }

    input.setAttribute("name", "obj");
    input.setAttribute("value", obj);
    form.appendChild(input);
    document.querySelector("div[class='test']").appendChild(form);
    form.submit();
}


