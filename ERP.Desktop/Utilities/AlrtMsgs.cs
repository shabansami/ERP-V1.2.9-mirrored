using System.Windows.Forms;

namespace ERP.Desktop.Utilities
{
    //Created By eng:Shaban Sami
    public class AlrtMsgs
    {
        public static void ShowCustomSuccess(string message, string header)
        {
            MessageBox.Show(message, header, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static DialogResult CheckDelete()
        {
           return MessageBox.Show("هل تريد الحذف؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
        }
        public static DialogResult CheckDelete(string entity)
        {
           return MessageBox.Show($"هل تريد حذف {entity} ؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
        }
        public static void EnterVaildData()
        {
            MessageBox.Show("تأكد من ادخال البيانات بشكل صحيح", "رسالة خطــأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void EnterVaildData(string dataName)
        {
            MessageBox.Show($"تأكد من ادخال {dataName} بشكل صحيح", "رسالة خطــأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void ChooseVaildData()
        {
            MessageBox.Show("تأكد من اختيار البيانات بشكل صحيح", "رسالة خطــأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void ChooseVaildData(string dataName)
        {
            MessageBox.Show($"تأكد من اختيار {dataName} بشكل صحيح", "رسالة خطــأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void ErrorWhenExcute()
        {
            MessageBox.Show("حدث خطأ أثناء تنفيذ العملية Error", "رسالة خطــأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void ErrorWhenExcute(string message)
        {
            MessageBox.Show("حدث خطأ أثناء تنفيذ العملية \n" + message, "رسالة خطــأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void DataDuplicate()
        {
            MessageBox.Show("تم ادخال البيان مسبقا .. بيان مكرر", "رسالة خطــأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void DataDuplicate(string data)
        {
            MessageBox.Show($"تم ادخال {data} مسبقا .. مكرر", "رسالة خطــأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void SaveSuccess()
        {
            MessageBox.Show("تم الحفظ بنجاح", "رسالة حفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void SaveSuccess(string additionalLine)
        {
            MessageBox.Show("تم الحفظ بنجاح\n" + additionalLine, "رسالة حفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void UpdateSuccess()
        {
            MessageBox.Show("تم التعديل بنجاح", "رسالة تعديل", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void DeleteSuccess()
        {
            MessageBox.Show("تم الحذف بنجاح", "رسالة حذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void ActivationSuccess()
        {
            MessageBox.Show("تم التفعيل بنجاح", "رسالة تفعيل", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void NoData()
        {
            MessageBox.Show("لا يوجد بيانات لعرضها", "رسالة خطــأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //By: Mohamed Ali
        public static void IncorrectUserNameOrPassword()
        {
            MessageBox.Show("خطأ في اسم المستخدم او كلمة المرور", "رسالة خطــأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static DialogResult OpenTransactionCheck(string fromTo = "")
        {
            return MessageBox.Show($"هل متأكد من فتح تحويل جديد {fromTo}؟", "تحويل جديد", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
        }
        public static void ShowMessageError(string msg)
        {
            MessageBox.Show(msg, "رسالة خطــأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
