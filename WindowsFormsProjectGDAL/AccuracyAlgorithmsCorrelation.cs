using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsProjectGDAL
{
    class AccuracyAlgorithmsCorrelation
    {
        private Info info;                      //коэффициенты аффинного преобразования

        private MyPoint pointModel;             //координаты начальной особой точки по эталону на исходном изображении
        private MyPoint pointModelTransform;    //координаты начальной особой точки по эталону на преобразованном изображении

        private List<Data> data = new List<Data>();     //список сохраненных координат, на основе которых собирается статистика

        public AccuracyAlgorithmsCorrelation(Info info)
        {
            this.info = info;

            pointModel = new MyPoint();
            pointModelTransform = new MyPoint();
        }

        //обратное аффинное преобразование
        private MyPoint reverseAffineTransform(MyPoint p)
        {
            MyPoint newPoint = new MyPoint();

            newPoint.x = (p.x * info.b2 - info.b2 * info.a0 - p.y * info.a2 + info.a2 * info.b0) /
                (info.a1 * info.b2 - info.b1 * info.a2);

            newPoint.y = (info.a1 * p.y - info.a1 * info.b0 - info.b1 * p.x + info.a0 * info.b1) /
                (info.a1 * info.b2 - info.b1 * info.a2);

            return newPoint; 
        }

        //расчет координат начальной особой точки и их сохранение на исходном и преобразованном изображении 
        public void setPointModel(Rectangle rect)
        {
            pointModel.x = ((double)(2 * rect.X + rect.Width)) / 2d;
            pointModel.y = ((double)(2 * rect.Y + rect.Height)) / 2d;

            pointModelTransform = reverseAffineTransform(pointModel);
        }

        //добавление статистики в коллекцию
        public void add(MyPoint k, MyPoint t)
        {
            data.Add(new Data(k, t));
        }

        //очистка данных
        public void clear()
        {
            data.Clear();
        }

        //получение Count 
        public double getCount()
        {
            return data.Count;
        }

        //получение Sx 
        public double getSx()
        {
            double Sx = 0d;

            foreach (Data d in data)
            {
                Sx += d.getK().x - d.getT().x;
            }
            Sx /= (data.Count * (data.Count - 1));

            return Sx;
        }

        //получение Sy 
        public double getSy()
        {
            double Sy = 0d;

            foreach (Data d in data)
            {
                Sy += d.getK().y - d.getT().y;
            }
            Sy /= (data.Count * (data.Count - 1));

            return Sy;
        }

        //получение ско
        public double getSko()
        {
            double Sx = 0d;
            double Sy = 0d;
            double sko = 0d;

            foreach (Data d in data)
            {
                Sx += d.getK().x - d.getT().x;
            }
            Sx /= (data.Count * (data.Count - 1));

            foreach (Data d in data)
            {
                Sy += d.getK().y - d.getT().y;
            }
            Sy /= (data.Count * (data.Count - 1));

            sko = Math.Sqrt(Sx * Sx + Sy * Sy);

            return sko;
        }

        public MyPoint getPointModel()
        {
            return pointModel;
        }

        public MyPoint getPointModelTransform()
        {
            return pointModelTransform;
        }
    }
}
