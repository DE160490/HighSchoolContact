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
    await updateSchoolYear();

    await fetchDataGetGrades();
    await updateGrade();

    await fetchDataGetClasses();
    await updateClasses();

    await updateWeek();

    await fetchDataGetSubjectTeacher();
    await fetchDataGetSubjects();
    await fetchDataGetPersonInformation();
    //viewSchedule('SCHE000056', 'CSID000073', 'HSE0000003', 'Chemistry', '07:15:00', '08:00:00', 'Lớp 10A1', 'Nguyễn Văn Quyết', 'Hóa');
    //await console.log(Grades);
    //await console.log(Classes);
    /*await console.log(SubjectTeachers);*/
    //await console.log(Subjects);
    //await console.log(PersonInformations);
};


main();


async function updateSchoolYear() {
    const select = document.querySelector("select[name='schoolYear']");
    const date = new Date();

    for (const schoolYear of SchoolYears) {
        const option = document.createElement('option');
        option.value = schoolYear.SchoolYearID;
        option.textContent = schoolYear.DateStart.split('-')[0] + '-' + schoolYear.DateEnd.split('-')[0];
        if (new Date(schoolYear.DateStart.split('T')[0]).getFullYear() <= date.getFullYear() && new Date(schoolYear.DateEnd.split('T')[0]).getFullYear() >= date.getFullYear()) {
            option.setAttribute("selected", true);
        }
        select.appendChild(option);
    }
}

async function updateGrade() {

    const schoolYearID = document.querySelector("select[name='schoolYear']").value;
    const selectgradeID = document.querySelector("select[name='gradeID']");

    const options = selectgradeID.querySelectorAll('option');
    options.forEach((option) => option.remove());

    let count = 0;

    for (const grade of Grades.filter(item => item.SchoolYearID === schoolYearID)) {
        const option = document.createElement('option');
        option.value = grade.GradeID;
        option.textContent = grade.GradeName;
        if (count === 0) {
            option.setAttribute("selected", true);
            count++;
        }
        selectgradeID.appendChild(option);
    }
    await updateWeek();
    await updateClasses()
}

async function updateClasses() {

    const gradeID = document.querySelector("select[name='gradeID']").value;
    const selectclassID = document.querySelector("select[name='classID']");

    const options = selectclassID.querySelectorAll('option');
    options.forEach((option) => option.remove());

    let count = 0;

    for (const classs of Classes.filter(item => item.GradeID === gradeID)) {
        const option = document.createElement('option');
        option.value = classs.ClassID;
        option.textContent = classs.ClassName;
        if (count === 0) {
            option.setAttribute("selected", true);
            count++;
        }
        selectclassID.appendChild(option);
    }
}

async function updateWeek() {
    const schoolYearID = document.querySelector("select[name='schoolYear']").value;
    const selectweekBegin = document.querySelector("select[name='weekBegin']");

    const options = selectweekBegin.querySelectorAll('option');
    options.forEach((option) => option.remove());

    var datenow = new Date();

    for (let i = 0; i < 10; i++) {
        if (datenow.getDay() !== 1) {
            datenow = new Date(datenow.getTime() - (24 * 60 * 60 * 1000));
        }
    }

    for (const schoolYear of SchoolYears) {
        if (schoolYear.SchoolYearID === schoolYearID) {

            console.log(schoolYear.DateStart + "  -  " + schoolYear.DateEnd);
            var start = new Date(schoolYear.DateStart.split('T')[0]);
            var end = new Date(schoolYear.DateEnd.split('T')[0]);

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
                selectweekBegin.appendChild(option);

            }
        }
    }
}

async function convertTimeFormat1(time) {
    var convert = "";
    if (time.getDate() <= 9) {
        if (time.getMonth() < 9) {
            convert = "0" + time.getDate() + "/" + "0" + (time.getMonth()+1);
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

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    async function closeForm() {
        var classdiv = document.querySelector("div[class='view_timetable']");
        var swap_formdiv = document.querySelector("div[name='swap_form']");
        classdiv.removeChild(swap_formdiv);
    }


    async function setTeacherinput(name) {
        var inputteacherName = document.querySelector("input[name='teacherNameEdit']");
        inputteacherName.setAttribute("value", name);
        inputteacherName.textContent = name;

        var label = document.querySelector("label[name='labelSearchTeacher']");
        var ul = document.querySelector("ul[name='searchTeacher']");

        if (ul != null) {
            label.removeChild(ul);
        }
    }

    async function setTeacherli(value, name) {
        var inputTeacherID = document.querySelector("input[name='teacherIDEdit']");
        inputTeacherID.setAttribute("value", value);
        setTeacherinput(name);
    }

    async function searchTeacher(classID) {
        var label = document.querySelector("label[name='labelSearchTeacher']");
        var ul = document.querySelector("ul[name='searchTeacher']");
        if (ul != null) {
            label.removeChild(ul);
        }

        const teacherName = document.querySelector("input[name='teacherNameEdit']").value;
        const filteredPersons = PersonInformations.filter((person) => person.Fullname.startsWith(teacherName));

        var newul = document.createElement('ul');
        newul.setAttribute("name", "searchTeacher");

        filteredPersons.forEach((person) => {
            const filteredTeacher = SubjectTeachers.filter((t) => t.TeacherID == person.ID && t.ClassID == classID);

            filteredTeacher.forEach(e => {
                var li = document.createElement('li');
                li.setAttribute("value", e.TeacherID);
                li.setAttribute("onclick", "setTeacherli('" + e.TeacherID + "','" + person.Fullname + "')");
                li.textContent = person.Fullname;
                newul.appendChild(li);
            });

        });
        //   console.log(newul)
        label.appendChild(newul);
    }

    async function setSubjectinput(name) {
        var inputsubjectName = document.querySelector("input[name='subjectNameEdit']");
        inputsubjectName.setAttribute("value", name);
        inputsubjectName.textContent = name;

        var label = document.querySelector("label[name='labelSearchSubject']");
        var ul = document.querySelector("ul[name='searchSubject']");

        if (ul != null) {
            label.removeChild(ul);
        }
    }

    async function setSubjectli(value, name) {
        var inputSubjectID = document.querySelector("input[name='subjectIDEdit']");
        inputSubjectID.setAttribute("value", value);
        setSubjectinput(name);
    }

    async function searchSubject(classID) {
        var label = document.querySelector("label[name='labelSearchSubject']");
        var ul = document.querySelector("ul[name='searchSubject']");

        if (ul != null) {
            label.removeChild(ul);
        }

        const subjectName = document.querySelector("input[name='subjectNameEdit']").value;

        const filteredSubject = Subjects.filter((subject) => subject.SubjectName.startsWith(subjectName));

        var newul = document.createElement('ul');
        newul.setAttribute("name", "searchSubject");

        filteredSubject.forEach((subject) => {
            const filtered = SubjectTeachers.filter((t) => t.SubjectID == subject.SubjectID && t.ClassID == classID);
            filtered.forEach(e => {
                var li = document.createElement('li');
                li.setAttribute("value", e.SubjectID);
                li.setAttribute("onclick", "setSubjectli('" + e.SubjectID + "','" + subject.SubjectName + "')");
                li.textContent = subject.SubjectName;
                newul.appendChild(li);
            })
        });

        label.appendChild(newul);
    }

    async function viewSchedule(scheduleID, classID, teacherID, subjectID, timeStart, timeEnd, className, teacherName, subjectName) {

        var classdiv = document.querySelector("div[class='view_timetable']");
        var formgroup = document.createElement('div');
        formgroup.setAttribute("name", "swap_form");
        formgroup.setAttribute("class", "swap_form");

        var form = document.createElement('form');
        var close = document.createElement('div');
        close.setAttribute("class", "close");
        var buttonClose = document.createElement('button');
        buttonClose.setAttribute("class", "btn_close");
        buttonClose.setAttribute("type", "button");
        buttonClose.setAttribute("onclick", "closeForm()");
        var iconClose = document.createElement('i');
        iconClose.setAttribute("class", "fa-regular fa-circle-xmark");
        buttonClose.appendChild(iconClose);
        close.appendChild(buttonClose);
        form.appendChild(close);

        //// Lớp Học
        var labelinput_form1 = document.createElement('label');
        labelinput_form1.setAttribute("class", "input_form");
        var span1 = document.createElement('span');
        span1.textContent = "Lớp học:";
        labelinput_form1.appendChild(span1);
        var input0_1 = document.createElement('input');
        input0_1.setAttribute("type", "text");
        input0_1.setAttribute("name", "classID");
        input0_1.setAttribute("value", classID);
        input0_1.setAttribute("hidden", "true");
        labelinput_form1.appendChild(input0_1);
        var input1 = document.createElement('input');
        input1.setAttribute("type", "text");
        input1.setAttribute("name", "className");
        input1.setAttribute("value", className);
        input1.setAttribute("placeholder", "Tên lớp học");
        input1.setAttribute("readonly", "true");
        labelinput_form1.appendChild(input1);

        //// Môn Học
        var labelinput_form2 = document.createElement('label');
        labelinput_form2.setAttribute("class", "input_form");
        labelinput_form2.setAttribute("name", "labelSearchSubject");
        var span2 = document.createElement('span');
        span2.textContent = "Môn học:";
        labelinput_form2.appendChild(span2);
        var input0_2 = document.createElement('input');
        input0_2.setAttribute("type", "text");
        input0_2.setAttribute("name", "subjectIDEdit");
        input0_2.setAttribute("value", subjectID);
        input0_2.setAttribute("hidden", "true");
        labelinput_form2.appendChild(input0_2);
        var input2 = document.createElement('input');
        input2.setAttribute("type", "text");
        input2.setAttribute("name", "subjectNameEdit");
        input2.setAttribute("value", subjectName);
        input2.setAttribute("oninput", "searchSubject('" + classID + "')");
        input2.setAttribute("placeholder", "Tên môn học");
        labelinput_form2.appendChild(input2);

        //// Giáo Viên
        var labelinput_form3 = document.createElement('label');
        labelinput_form3.setAttribute("class", "input_form");
        labelinput_form3.setAttribute("name", "labelSearchTeacher");
        var span3 = document.createElement('span');
        span3.textContent = "Giáo viên:";
        labelinput_form3.appendChild(span3);
        var input0_3 = document.createElement('input');
        input0_3.setAttribute("type", "text");
        input0_3.setAttribute("name", "teacherIDEdit");
        input0_3.setAttribute("value", teacherID);
        input0_3.setAttribute("hidden", "true");
        labelinput_form3.appendChild(input0_3);
        var input3 = document.createElement('input');
        input3.setAttribute("type", "text");
        input3.setAttribute("name", "teacherNameEdit");
        input3.setAttribute("value", teacherName);
        input3.setAttribute("oninput", "searchTeacher('" + classID + "')");
        input3.setAttribute("placeholder", "Họ tên giáo viên");
        labelinput_form3.appendChild(input3);

        //// Thời Gian Bắt Đầu
        var labelinput_form4 = document.createElement('label');
        labelinput_form4.setAttribute("class", "input_form");
        var span4 = document.createElement('span');
        span4.textContent = "Thời gian bắt đầu:";
        labelinput_form4.appendChild(span4);
        var input4 = document.createElement('input');
        input4.setAttribute("type", "time");
        input4.setAttribute("name", "timeStart");
        input4.setAttribute("value", timeStart);
        labelinput_form4.appendChild(input4);

        //// Thời Gian Kết Thúc
        var labelinput_form5 = document.createElement('label');
        labelinput_form5.setAttribute("class", "input_form");
        var span5 = document.createElement('span');
        span5.textContent = "Thời gian kết thúc:";
        labelinput_form5.appendChild(span5);
        var input5 = document.createElement('input');
        input5.setAttribute("type", "time");
        input5.setAttribute("name", "timeEnd");
        input5.setAttribute("value", timeEnd);
        labelinput_form5.appendChild(input5);

        //// Button Edit với Delete
        var btn_group = document.createElement('div');  
        btn_group.setAttribute("class", "btn_group");
        var btn_edit = document.createElement('button');
        btn_edit.setAttribute("class", "edit");
        btn_edit.setAttribute("name", "edit");
        btn_edit.setAttribute("type", "submit");
        btn_edit.setAttribute("onclick", "editSchedule('" + scheduleID + "')");
        btn_edit.textContent = "Chỉnh sửa";
        btn_group.appendChild(btn_edit);
        var btn_delete = document.createElement('button');
        btn_delete.setAttribute("class", "delete");
        btn_delete.setAttribute("name", "delete");
        btn_delete.setAttribute("type", "submit");
        btn_delete.setAttribute("onclick", "deleteSchedule('" + scheduleID + "')");
        btn_delete.textContent = "Xóa";
        btn_group.appendChild(btn_delete);

        //// Add thẻ vào html
        form.appendChild(labelinput_form1);
        form.appendChild(labelinput_form2);
        form.appendChild(labelinput_form3);
        form.appendChild(labelinput_form4);
        form.appendChild(labelinput_form5);
        form.appendChild(btn_group);
        formgroup.appendChild(form);
        classdiv.appendChild(formgroup);
    }

    async function editSchedule(scheduleID) {
        var teacherID = document.querySelector("input[name='teacherIDEdit']");
        var subjectID = document.querySelector("input[name='subjectIDEdit']");
        var timeStart = document.querySelector("input[name='timeStart']");
        var timeEnd = document.querySelector("input[name='timeEnd']");

        var schedule = scheduleID + "$" + teacherID.value + "$" + subjectID.value + "$" + timeStart.value + "$" + timeEnd.value;

           var form = document.createElement("form");
             var input = document.createElement("input");
                 input.setAttribute("name", "scheduleEdit");
                 input.setAttribute("value", schedule);

             form.setAttribute("method", "post");
             form.setAttribute("asp-controller", "Admin");
             form.setAttribute("asp-action", "UpdateTimeTable");
             form.appendChild(input);
        var classdiv = document.querySelector("div[class='view_timetable']");
        classdiv.appendChild(form);
             //form.submit();
    }

async function deleteSchedule(scheduleID) {
        console.log("Delete Schedule JS");
        var form = document.createElement("form");
        form.setAttribute("method", "post");
        form.setAttribute("asp-controller", "Admin");
        form.setAttribute("asp-action", "DeleteTimeTable");
            var input = document.createElement("input");
                input.setAttribute("name", "scheduleDelete");
                input.setAttribute("value", scheduleID);
        form.appendChild(input);
    var classdiv = document.querySelector("div[class='view_timetable']");
    classdiv.appendChild(form);
        //form.submit();
    }