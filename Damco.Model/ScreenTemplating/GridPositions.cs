using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.ScreenTemplating
{
    /// <summary>
    /// Defines viewport of the window.
    /// </summary>
    /// <remarks>
    /// A new boolean property which is used to check if the control 
    /// is to be placed in a new row.
    /// </remarks>
    public class GridPositions
    {
        /// <summary>
        /// Indicates an ExtraSmall viewport.
        /// </summary>
        public GridPosition ExtraSmall { get; set; } 

        /// <summary>
        /// Indicates an Small viewport.
        /// </summary>
        public GridPosition Small { get; set; }

        /// <summary>
        /// Indicates a Medium viewport.
        /// </summary>
        public GridPosition Medium { get; set; } 

        /// <summary>
        /// Indicates a Large viewport.
        /// </summary>
        public GridPosition Large { get; set; } 

        /// <summary>
        /// Indicates if the template is to be started on a new row.
        /// </summary>
        public bool StartOnNewRow { get; set; }
    }

    /// <summary>
    /// Defines Bootstrap property of a grid layout.
    /// </summary>
    public class GridPosition
    {
        /// <summary>
        /// A boolean property which illustrates if the control is to be visible for a viewport.
        /// </summary>
        /// <remarks>
        /// If a control must be visible for an ExtraSmall screen, then the Bootstrap Class is formed as - "visible-xs".
        /// </remarks>
        public bool Visible { get; set; }

        /// <summary>
        /// A boolean property which illustrates if the control is to be hidden for a viewport.
        /// </summary>
        /// <remarks>
        /// If a control must be hidden for an ExtraSmall screen, then the Bootstrap Class is formed as - "hidden-xs".
        /// </remarks>
        public bool Hidden { get; set; } 

        /// <summary>
        /// Number of columns to be occupied by the control in a grid layout for a viewport.
        /// </summary>
        /// <remarks>
        /// If a control must occupy 5 columns in an ExtraSmall screen, then the Bootstrap Class is formed as - "col-xs-5".
        /// </remarks>
        public int? NumberOfColumns { get; set; }

        /// <summary>
        /// Number of columns to be pushed away right in a grid layout.
        /// </summary>
        /// <remarks>
        /// If a control is to be pushed away 5 columns in a grid layout in an ExtraSmall screen, then the Bootstrap Class is formed as - "col-xs-push-5"
        /// </remarks>
        public int? MoveToRight { get; set; }

        /// <summary>
        /// Number of columns to be pulled left in a grid layout.
        /// </summary>
        /// <remarks>
        /// If a control is to be pulled 5 columns in a grid layout in an ExtraSmall screen, then the Bootstrap Class is formed as - "col-xs-pull-5"
        /// </remarks>
        public int? MoveToLeft { get; set; } 

        /// <summary>
        /// Moves columns to right using left margin.
        /// </summary>
        /// <remarks>
        /// For example : col-xs-offset-5 , increases left margin by 5 columns.
        /// </remarks>
        public int? Offset { get; set; }

        /// <summary>
        /// Height of the control on a viewport.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Height of the control to be deducted from windowHeight (where Height is measured after MenuHeight on the Window).
        /// </summary>
        public int? HeightOffset { get; set; }

        /// <summary>
        /// Percentage of the heightOffset to be used by the control as height.
        /// </summary>
        /// <remarks>
        /// Value lies between 0 and 1.
        /// </remarks>
        public double Factor { get; set; } = 1;

        /// <summary>
        /// The minimum height of the control when no height is defined.
        /// </summary>
        public int? MinHeight { get; set; }

        public HeightDeterminationType HeightDeterminationType { get; set; }
        public int? MaxHeight { get; set; }
        
    }

    public enum HeightDeterminationType
    {
        Undefined = 0,
        GrowBasedOnContents = 1,
        FixedHeight = 2,
        BasedOnScreenSize = 3,
        BasedOnScreenSizeAndStartingPosition = 4
    }
}
