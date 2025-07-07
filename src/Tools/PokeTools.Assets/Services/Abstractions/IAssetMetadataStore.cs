using PokeCore.Common.Results;
using PokeCore.IO;
using PokeTools.Assets.Core;

namespace PokeTools.Assets.Services.Abstractions;

public interface IAssetMetadataStore
{
    /// <summary>
    /// Vérifie qu'une métadonnée associée au chemin de l'asset spécifié <paramref name="assetPath"/> existe.
    /// </summary>
    /// <param name="assetPath">Le chemin de l'asset associé à la métadonnée/param>
    /// <returns><c>true</c> if a metadata exists for this asset; <c>false</c> otherwise</returns>
    Task<bool> ExistsAsync(VirtualPath assetPath);

    /// <summary>
    /// Charge ou crée les métadonnées associée au chemin de l'asset spécifié <paramref name="assetPath"/>
    /// </summary>
    /// <remarks>
    /// Cette méthode crée une métadonnée si aucune métadonnée n'existe pour cet asset
    /// </remarks>
    /// <param name="assetPath">Le chemin de l'asset associé à la métadonnée</param>
    /// <returns>Le résultat de l'opération. Si tout s'est bien passé, les métadonnées chargée ou créée; Sinon l'erreur</returns>
    Task<Result<AssetMetadata>> GetAsync(VirtualPath assetPath);

    /// <summary>
    /// Charge une métadonnée associée au chemin de l'asset spécifié <paramref name="assetPath"/>
    /// </summary>
    /// <param name="assetPath">Le chemin de l'asset associé à la métadonnée</param>
    /// <returns>Le résultat de l'opération. Si tout s'est bien passé, les métadonnées chargée; Sinon l'erreur</returns>
    Task<Result<AssetMetadata>> LoadAsync(VirtualPath assetPath);

    /// <summary>
    /// Sauvegarde la métadonnée <paramref name="metadata"/> au chemin de l'asset spécifié <paramref name="assetPath"/>
    /// </summary>
    /// <param name="assetPath">Le chemin de l'asset associé à la métadonnée</param>
    /// <returns>Le résultat de l'opération. En cas d'erreur, un message d'erreur lisible</returns>
    Task<Result<Unit>> SaveAsync(AssetMetadata metadata, VirtualPath assetPath);

    /// <summary>
    /// Crée une métadonnée associée au chemin de l'asset spécifié <paramref name="assetPath"/>
    /// </summary>
    /// <param name="assetPath">Le chemin de l'asset associé à la métadonnée</param>
    /// <returns>Le résultat de l'opération. En cas de succès, la métadonnée créée; Autrement, un message d'erreur lisible</returns>
    Task<Result<AssetMetadata>> CreateAsync(VirtualPath assetPath);
}